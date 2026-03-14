using System.Threading;

namespace pr.net.Services;

// we force keys to expire so they can be hotloaded
public class ExternalAuthService { 
    // repo token
    private string _repoToken = string.Empty; 
    private bool _repoTokenExpired { 
        get => _repoTokenExpired; 
        set {
            _repoTokenTimer?.Dispose();
            if(!value)
                _repoTokenTimer = new Timer(_ => _repoTokenExpired = true, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
        }
    }

    private System.Threading.Timer? _repoTokenTimer;
    private readonly Object _repoLock = new();

    // chat token
    private string _chatToken = string.Empty;

    private bool _chatTokenExpired { 
        get => _chatTokenExpired; 
        set {
            _chatTokenTimer?.Dispose();
            if(!value)
                _chatTokenTimer = new Timer(_ => _chatTokenExpired = true, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
        }
    } 

    private System.Threading.Timer? _chatTokenTimer;
    private readonly Object _chatLock = new();

    public string GetRepoBearerToken(IConfiguration configuration) =>
        (_repoToken == null || _repoTokenExpired == true)
            ? RefreshRepoBearerToken(configuration)
            : _repoToken;
 
    private string RefreshRepoBearerToken(IConfiguration configuration) {
        lock(_repoLock) {
            if(!_repoTokenExpired && !string.IsNullOrWhiteSpace(_repoToken))
                return _repoToken;

            _repoTokenExpired = false; 
            return _repoToken = System.Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN") 
                ?? throw new InvalidOperationException("PR_NET_REPO_TOKEN environment variable not found.");
        }
    }  

    public string GetChatToken(IConfiguration configuration) =>
        string.IsNullOrWhiteSpace(_chatToken)
            ? RefreshChatToken(configuration)
            : _chatToken;

    public string RefreshChatToken(IConfiguration configuration) {
        lock(_chatLock) {
            if(!_chatTokenExpired && !string.IsNullOrWhiteSpace(_chatToken))
                return _chatToken;

            _chatTokenExpired = false;
            return _chatToken = System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN")
                ?? throw new InvalidOperationException("PR_NET_CHAT_TOKEN environment variable not found.");
        };
    }

}