using System.Text;

namespace pr.net.Services;

public class AuthService {

    // repo's token expires, refresh it intermittently 
    private string _repoBearerToken = string.Empty;
    private System.Threading.Timer? _repoTokenTimer;
    private bool _repoTokenExpired = true;
    private bool RepoTokenExpired { 
        get => _repoTokenExpired; 
        set {
            _repoTokenTimer?.Dispose();
            if(!RepoTokenExpired)
                _repoTokenTimer = new Timer(_ => _repoTokenExpired = true, null, TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan);
        }
    }

    public string GetRepoBearerToken(IConfiguration configuration) =>
        (_repoBearerToken == null || _repoTokenExpired == true)
            ? RefreshBearerToken(configuration)
            : _repoBearerToken;

    public string RefreshBearerToken(IConfiguration configuration) {
        RepoTokenExpired = false; 
        return System.Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN") 
            ?? throw new InvalidOperationException("PR_NET_REPO_TOKEN environment variable not found.");
    }

   private string _chatToken = string.Empty;  

    public string GetChatToken(IConfiguration configuration) =>
        string.IsNullOrWhiteSpace(_chatToken)
            ? RefreshChatBearerToken(configuration)
            : _chatToken;

    public string RefreshChatBearerToken(IConfiguration configuration) =>
        _chatToken = string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN"))
                ? throw new InvalidOperationException("PR_NET_CHAT_TOKEN environment variable not found.")
                : System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN")!;
                /*Convert.ToBase64String(
            Encoding.ASCII.GetBytes(
                string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN"))
                ? throw new InvalidOperationException("PR_NET_CHAT_TOKEN environment variable not found.")
                : System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN")!
            )
        );  */
    
}