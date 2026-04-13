using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Options;

using pr.net.Models.Patterns;

using pr.net.Configurations.Repo;

namespace pr.net.Services.Patterns;

public class LocalPatternService : IPatternService { 
    private readonly ILogger<LocalPatternService> _logger; 

    private static readonly string _dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Patterns");
    private static readonly string _patternPath = Path.Combine(_dirPath, @"user-cache.json");
    private static readonly string _backupPath = Path.Combine(_dirPath, @"user-cache-backup.json");
    private static readonly string _tempPath = Path.Combine(_dirPath, @"user-cache-temp.json");

    private SemaphoreSlim _patternLock = new(1, 1); // global for all functions within service.

    private int _capacity; // max number of patterns per user.
    private int _backupCounter; // used to track how many writes have occurred, when it hits 0 we went to persist the json to the backup file.  

    private Dictionary<string, LinkedList<Pattern>> _patternHash = []; // key: userid, value: patterns for specified userid.
    private Dictionary<string, int> _patternCounterHash = []; // key: userid, value: highest pattern id.

    private readonly List<string>? _configuredUsers;  

    public LocalPatternService(
        IOptions<RepoConfiguration> configuration, 
        ILogger<LocalPatternService> logger
    ) {
        _logger = logger;

        _configuredUsers = configuration.Value.Users?.AuthorizedUsers;
        if(_configuredUsers == null)
            _logger.LogWarning($"No authorized users were found in {nameof(LocalPatternService)}.");

        // backup interval == length of pattern history.
        _backupCounter = _capacity = configuration.Value.Users?.Patterns.PatternHistoryLength 
            ?? throw new InvalidOperationException($"Pattern capacity could not be fetched from configuration in {nameof(LocalPatternService)}");

        // form the in memory list based on the file and append found configured users.
        // keep in mind that the system may not have user filtering enabled, and new patterns from unconfigured users still need to be appended-
        // do not implement user authorization logic here, just append and let that logic live where it needs to live.
        List<UserPatterns>? userPatterns;
        if(File.Exists(_patternPath)) {
            try {
                userPatterns = JsonSerializer.Deserialize<List<UserPatterns>>(File.ReadAllText(_patternPath)) ?? [];
            } catch(Exception exception) {
                throw new InvalidOperationException($"Failed to deserialize user-cache.json file in {nameof(LocalPatternService)}, try reverting to backup file.", exception); 
            }
        } else {
            Directory.CreateDirectory(_dirPath);
            using (File.Create(_patternPath));
            userPatterns = [];
        }

        using (File.Create(_tempPath));

        // if user hasn't been added to the list yet, append them.
        if(_configuredUsers != null)
            foreach(string id in _configuredUsers)
                if(!userPatterns.Any(p => p.UserId == id))
                    userPatterns.Add(new UserPatterns() { UserId = id }); // append to list { userid, empty pattern list }.
        
        // patterns are stored as a dictionary of key: userid, value: linked list of patterns. (lru cache-esque implementation).
        // indices are tracked via a dictionary of key: userid, value: index count.
        foreach(UserPatterns userPattern in userPatterns) {
            _patternHash.TryAdd(userPattern.UserId, new LinkedList<Pattern>(userPattern.History));
            _patternCounterHash.TryAdd(userPattern.UserId, userPattern.History.Count > 0 ? userPattern.History.Max(p => p.Id) : 0); // counter == highest id to avoid dupes.
        }

        try {
            // if the backup doesn't exist yet, just go ahead and capture whatever data we have.
            if(!File.Exists(_backupPath))
                File.Copy(_patternPath, _backupPath, overwrite: true);

            // atomically persist our pattern file (store the list, not the hash table).
            File.WriteAllText(_tempPath, JsonSerializer.Serialize(userPatterns));
            File.Copy(_tempPath, _patternPath, overwrite: true);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Failed to serialize and store user patterns in {nameof(LocalPatternService)}, try reverting to backup file.", exception); 
        }
    }

    // if the user's patterns exist, return them, else append the user to the hash and return their new instance.
    public async ValueTask<IEnumerable<Pattern>> TouchUserPatterns(string userId, CancellationToken cancellationToken) { 
        await _patternLock.WaitAsync(cancellationToken);
        try {
            if(!_patternHash.TryGetValue(userId, out LinkedList<Pattern> patterns)) {
                return patterns 
                    ?? throw new InvalidOperationException($"Given patterns from in memory cache was null in {nameof(TouchUserPatterns)}"); // this shouldn't technically be possible, but just to be safe.
            } else { 
                if(--_backupCounter <= 0)
                    BackupPatterns();
                _patternCounterHash.TryAdd(userId, 0);
                _patternHash.TryAdd(userId, new LinkedList<Pattern>());
                // persist to file anytime a user is added.
                PersistPatterns(); 
            };

            return [.._patternHash[userId]];
        } finally { 
            _patternLock.Release();
        }
    }

    // if a pattern is touched, it gets pushed to be the most recent instance in the linked list.
    public async ValueTask<bool> TouchPattern(string userId, int patternId, CancellationToken cancellationToken) {  
        await _patternLock.WaitAsync(cancellationToken); // avoid multiple mutators.   
        try { 
            if(!_patternHash.TryGetValue(userId, out LinkedList<Pattern>? patterns)) {
                _logger.LogError($"Provided user ID mapping could not be found in {nameof(TouchPattern)}");
                return false;
            } 

            // short circuit if value is already most recently used.
            if(patterns.First?.Value.Id == patternId)
                return true;  

            Pattern? pattern = patterns.FirstOrDefault(p => p.Id == patternId);
            if(pattern == null) {
                _logger.LogError($"Pattern ID provided could not be found in {nameof(TouchPattern)}");
                return false;
            }

            // backup cache before persisting so a safe value is backed up.
            if(--_backupCounter <= 0)
                BackupPatterns();

            if(!patterns.Remove(pattern)) {
                _logger.LogError($"Failed to remove pattern by ID in {nameof(TouchPattern)}");
                return false;
            }

            patterns.AddFirst(pattern);
            PersistPatterns();

            return true;
        } finally {
            _patternLock.Release();
        }
    }

    public async ValueTask<bool> AddPattern(string userId, Pattern pattern, CancellationToken cancellationToken) { 
        await _patternLock.WaitAsync(cancellationToken); // avoid multiple callers.
        try {
            if(--_backupCounter <= 0)
                BackupPatterns();

            if(!_patternCounterHash.ContainsKey(userId))
                _patternCounterHash.TryAdd(userId, 0);
            pattern.Id = ++_patternCounterHash[userId]; 

            if(_patternHash.TryGetValue(userId, out LinkedList<Pattern>? patterns)) {
                patterns.AddFirst(pattern); 
                if(patterns.Count > _capacity)
                    patterns.RemoveLast();
                PersistPatterns();
                return true;
            } else {
                if(_patternHash.TryAdd(userId, new LinkedList<Pattern>([pattern]))) {
                    PersistPatterns();
                    return true;
                }
                _logger.LogError($"Failed to add pattern in {nameof(AddPattern)}");
                return false;
            }
        } finally {
            _patternLock.Release();
        }
    }

    // this method must be accessed inside of a mutex.
    private void PersistPatterns() {
        string validatedData = ValidateCacheData();
        try {
            File.WriteAllText(_tempPath, validatedData);
            File.Copy(_tempPath, _patternPath, overwrite: true);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Could not persist data to user-cache.json in {nameof(PersistPatterns)}.", exception);
        }
    }

    // this method must be accessed inside of a mutex.
    private void BackupPatterns() { 
        _backupCounter = _capacity;
        string validatedData = ValidateFileData(_patternPath);  
        try {
            File.WriteAllText(_tempPath, validatedData);
            File.Copy(_tempPath, _backupPath, overwrite: true); 
        } catch(Exception exception) {
            throw new InvalidOperationException($"Could not backup user-cache.json in {nameof(BackupPatterns)}.", exception);
        } 
    }

    // this function must be accessed inside of a mutex.
    private string ValidateCacheData() {
        string checkString = string.Empty;
        try {  
            List<UserPatterns> currentPatterns = [.._patternHash.Select(p => new UserPatterns() { UserId = p.Key, History = [..p.Value] })];
            checkString = JsonSerializer.Serialize(currentPatterns);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Patterns in user-cache.json appear to be invalid, could not serialize json in {nameof(ValidateCacheData)}.", exception);
        } 

        return checkString;
    }

    // this function must be accessed inside of a mutex.
    private string ValidateFileData(string path) {
        List<UserPatterns>? checkData = null;
        string checkString = string.Empty;
        try {  
            checkData = JsonSerializer.Deserialize<List<UserPatterns>>(File.ReadAllText(path));
            checkString = JsonSerializer.Serialize(checkData);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Patterns from fetched file ${path} appear to be invalid in {nameof(ValidateFileData)}.", exception);
        }

        if(checkData == null || string.IsNullOrWhiteSpace(checkString))
            throw new InvalidOperationException($"Unexpected error encountered validating file data from {path}, could not deserialize or serialize json in {nameof(ValidateFileData)}.");
        
        return checkString;
    }

}