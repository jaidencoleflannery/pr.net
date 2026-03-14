using System.Threading;

namespace pr.net.Services.Tokens;

// we force keys to expire so they can be hotloaded
public class TokenService : ITokenService { 
    // repo token
    private string _repoToken = string.Empty; 
    private bool _repoTokenExpired = true;
    private bool RepoTokenExpired { 
        get => _repoTokenExpired; 
        set {
            _repoTokenTimer?.Dispose();
            if(!value) {
                _repoTokenExpired = value;
                _repoTokenTimer = new Timer(_ => _repoTokenExpired = true, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
            }
        }
    }

    private System.Threading.Timer? _repoTokenTimer;
    private readonly Object _repoLock = new();

    // chat token
    private string _chatToken = string.Empty;

    private bool _chatTokenExpired = true;
    private bool ChatTokenExpired { 
        get => _chatTokenExpired; 
        set {
            _chatTokenTimer?.Dispose();
            if(!value) {
                _chatTokenExpired = value;
                _chatTokenTimer = new Timer(_ => _chatTokenExpired = true, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
            }
        }
    } 

    private System.Threading.Timer? _chatTokenTimer;
    private readonly Object _chatLock = new();

    public ValueTask<string> GetRepoTokenAsync() =>
        ValueTask.FromResult((string.IsNullOrWhiteSpace(_repoToken) || RepoTokenExpired == true)
            ? RefreshRepoToken()
            : _repoToken);
 
    private string RefreshRepoToken() {
        lock(_repoLock) {
            if(!RepoTokenExpired && !string.IsNullOrWhiteSpace(_repoToken))
                return _repoToken;

            RepoTokenExpired = false; 
            return _repoToken = System.Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN") 
                ?? throw new InvalidOperationException("PR_NET_REPO_TOKEN environment variable not found.");
        }
    }  

    // async so that interface can be dynamic
    public ValueTask<string> GetChatTokenAsync() =>
        ValueTask.FromResult(string.IsNullOrWhiteSpace(_chatToken) || _chatTokenExpired
            ? RefreshChatToken()
            : _chatToken);

    private string RefreshChatToken() {
        lock(_chatLock) {
            if(!ChatTokenExpired && !string.IsNullOrWhiteSpace(_chatToken))
                return _chatToken;

            ChatTokenExpired = false;
            return _chatToken = System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN")
                ?? throw new InvalidOperationException("PR_NET_CHAT_TOKEN environment variable not found.");
        };
    }

}