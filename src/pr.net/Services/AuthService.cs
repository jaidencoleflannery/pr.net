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
        return Convert.ToBase64String(
            Encoding.ASCII.GetBytes(
                $"{configuration["pr.net.RepoEmail"]}:{System.Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN")}"
            )
        ); 
    }

   private string _chatBearerToken = string.Empty;  

    public string GetChatBearerToken(IConfiguration configuration) =>
        (_chatBearerToken == null)
            ? RefreshChatBearerToken(configuration)
            : _chatBearerToken;

    public string RefreshChatBearerToken(IConfiguration configuration) =>
        _chatBearerToken = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(
                $"{System.Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN")}"
            )
        ); 
    
}