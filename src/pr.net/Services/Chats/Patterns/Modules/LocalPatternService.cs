using System.Text.Json;
using Microsoft.Extensions.Options;

using pr.net.Configurations.Repo;

using pr.net.Models.Patterns;

namespace pr.net.Services.Patterns;

public class LocalPatternService : IPatternService {

    Dictionary<string, LinkedList<Pattern>> hash = new(); // key: userid, value: patterns for specified userid.

    private IOptions<RepoConfiguration>? _configuration;
    private JsonSerializerOptions _options = new() { WriteIndented = true };

    private readonly List<string>? _configuredUsers;
    private Dictionary<string, UserPatterns> _patternHash = [];

    private static readonly string patternPath = "./Patterns/user-cache.json";
     
    public LocalPatternService(IOptions<RepoConfiguration> configuration) {
        _configuration = configuration;
        _configuredUsers = this._configuration.Value.Users?.AuthorizedUsers ??
            throw new InvalidOperationException($"Could not fetch authorized users from configuration in {nameof(LocalPatternService)}");

        // form the in memory list based on the file + any configured users.
        // keep in mind that the system may not have user filtering enabled, and new patterns from unconfigured users still need to be appended.
        List<UserPatterns>? userPatterns;
        if(File.Exists(patternPath))
            userPatterns = JsonSerializer.Deserialize<List<UserPatterns>>(File.ReadAllText("./Patterns/user-cache.json")) ?? [];
        else
            userPatterns = [];

        // if user hasn't been added to the list yet, append them.
        foreach(string id in _configuredUsers)
            if(!userPatterns.Any(p => p.UserId == id))
                userPatterns.Add(new UserPatterns());
        
        // hash each instance for O(1) lookup.
        foreach(UserPatterns pattern in userPatterns)
            _patternHash.TryAdd(pattern.UserId, pattern); 
    }

    public async Task<UserPatterns?> GetUserPatterns(string userId) =>
        _patternHash.TryGetValue(userId, out UserPatterns? patterns)
            ? patterns
            : null; 

    // if a pattern is touched, it gets pushed to be the most recent instance in the list.
    public async Task<bool> TouchPattern(UserPatterns pattern) {

    }

}