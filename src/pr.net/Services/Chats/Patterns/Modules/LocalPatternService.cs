using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Options;

using pr.net.Models.Patterns;

using pr.net.Configurations.Repo;

namespace pr.net.Services.Patterns;

public class LocalPatternService : IPatternService { 
    private readonly ILogger<LocalPatternService> _logger; 

    private ConcurrentDictionary<string, LinkedList<Pattern>> _patternHash = new(); // key: userid, value: patterns for specified userid.
    private SemaphoreSlim _patternLock = new(1, 1); // global for all functions within service.
    private int _capacity;

    private readonly List<string> _configuredUsers; 

    private static readonly string _dirPath = Path.Combine(@".", @"Patterns");
    private static readonly string _patternPath = Path.Combine(_dirPath, @"user-cache.json");
    private static readonly string _backupPath = Path.Combine(_dirPath, @"user-cache-backup.json");
    private static readonly string _tempPath = Path.Combine(_dirPath, @"user-cache-temp.json");

    private int _backupCounter; // used to track how many writes have occurred, when it hits 0 we went to persist the json to the backup file.
     
    public LocalPatternService(
        IOptions<RepoConfiguration> configuration, 
        ILogger<LocalPatternService> logger
    ) {
        _logger = logger;

        _configuredUsers = configuration.Value.Users?.AuthorizedUsers 
            ?? throw new InvalidOperationException($"Could not fetch authorized users from configuration in {nameof(LocalPatternService)}");

        _backupCounter = _capacity = configuration.Value.Users?.Patterns.PatternHistoryLength 
            ?? throw new InvalidOperationException($"Pattern capacity could not be fetched from configuration in {nameof(LocalPatternService)}");        

        // form the in memory list based on the file + any configured users.
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
        foreach(string id in _configuredUsers)
            if(!userPatterns.Any(p => p.UserId == id))
                userPatterns.Add(new UserPatterns() { UserId = id });
        
        // patterns are stored as a dictionary of key: userid, value: linked list of patterns (lru cache implementation).
        foreach(UserPatterns userPattern in userPatterns)
            _patternHash.TryAdd(userPattern.UserId, new LinkedList<Pattern>(userPattern.PatternHistory));

        try {
            // if the backup doesn't exist yet, just go ahead and capture whatever data we have.
            if(!File.Exists(_backupPath))
                File.Copy(_patternPath, _backupPath, overwrite: true);

            // atomically persist our pattern file
            File.WriteAllText(_tempPath, JsonSerializer.Serialize(userPatterns));
            File.Copy(_tempPath, _patternPath, overwrite: true);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Failed to serialize and store user patterns in {nameof(LocalPatternService)}, try reverting to backup file.", exception); 
        }
    }

    // if the user's patterns exist, return them, else make a new one and return it, else null if something went wrong.
    public async ValueTask<IEnumerable<Pattern>> TouchUserPatterns(string userId, CancellationToken cancellationToken) { 
        _patternHash.GetOrAdd(userId, _ => new LinkedList<Pattern>());  
        return [.._patternHash[userId]];
    }

    // if a pattern is touched, it gets pushed to be the most recent instance in the linked list.
    public async ValueTask<bool> TouchPattern(string userId, int patternId, CancellationToken cancellationToken) {  

        if(!_patternHash.TryGetValue(userId, out LinkedList<Pattern>? patterns)) {
            _logger.LogError($"Provided user ID mapping could not be found in {nameof(TouchPattern)}");
            return false;
        }

        await _patternLock.WaitAsync(cancellationToken); // avoid multiple mutators.  
        try { 
            // short circuit if value is already most recently used.
            if(patterns.First?.Value.Id == patternId)
                return true;

            if(_backupCounter-- <= 0)
                BackupPatterns();

            Pattern? pattern = patterns.FirstOrDefault(p => p.Id == patternId);
            if(pattern == null) {
                _logger.LogError($"Pattern ID provided could not be found in {nameof(TouchPattern)}");
                return false;
            }

            if(!patterns.Remove(pattern)) {
                _logger.LogError($"Failed to remove pattern by ID in {nameof(TouchPattern)}");
                return false;
            }

            patterns.AddFirst(pattern);
            return true;
        } finally {
            PersistPatterns();
            _patternLock.Release(); 
        }
    }

    public async ValueTask<bool> AddPattern(string userId, Pattern pattern, CancellationToken cancellationToken) { 
        await _patternLock.WaitAsync(cancellationToken); // avoid multiple callers.
        try {
            if(_backupCounter-- <= 0)
                BackupPatterns();

            if(_patternHash.TryGetValue(userId, out LinkedList<Pattern>? patterns)) {
                patterns.AddFirst(pattern); 
                if(patterns.Count > _capacity)
                    patterns.RemoveLast(); 
                return true;
            } else {
                if(_patternHash.TryAdd(userId, new LinkedList<Pattern>([pattern]))) { 
                    return true;
                }
                _logger.LogError($"Failed to add pattern in {nameof(AddPattern)}");
                return false;
            }
        } finally {
            PersistPatterns();
            _patternLock.Release(); 
        }
    }

    private void PersistPatterns() {
        string validatedData = ValidateCacheData();
        try {
            File.WriteAllText(_tempPath, validatedData);
            File.Copy(_tempPath, _patternPath, overwrite: true);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Could not persist data to user-cache.json in {nameof(PersistPatterns)}.", exception);
        }
    }

    private void BackupPatterns() {
        Interlocked.Exchange(ref _backupCounter, _capacity); 
        ValidateFileData(_patternPath); 
        try {
            File.Copy(_patternPath, _backupPath, overwrite: true);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Could not backup user-cache.json in {nameof(BackupPatterns)}.", exception);
        }    
    }

    private string ValidateCacheData() {
        string checkString = string.Empty;

        try {  
            List<UserPatterns> currentPatterns = [.._patternHash.Select(p => new UserPatterns() { UserId = p.Key, PatternHistory = [..p.Value] })];
            checkString = JsonSerializer.Serialize(currentPatterns);
        } catch(Exception exception) {
            throw new InvalidOperationException($"Patterns in user-cache.json appear to be invalid, could not serialize json in {nameof(ValidateCacheData)}.", exception);
        } 

        return checkString;
    }

    private List<UserPatterns> ValidateFileData(string path) {
        List<UserPatterns>? checkData = null;

        try {  
            checkData = JsonSerializer.Deserialize<List<UserPatterns>>(File.ReadAllText(path));
        } catch(Exception exception) {
            throw new InvalidOperationException($"Patterns in user-cache.json appear to be invalid, could not deserialize json in {nameof(ValidateFileData)}.", exception);
        }

        if(checkData == null)
            throw new InvalidOperationException($"Unexpected error encountered validating pattern data, it appears user-cache.json is empty, could not serialize json in {nameof(ValidateFileData)}.");

        return checkData;
    }

}