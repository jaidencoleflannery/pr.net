using System.Text;

namespace pr.net.Services;

public class AuthService {

    private string _bearerToken = string.Empty;
    private System.Threading.Timer? _timer;
    private bool _expired = true;
    private bool Expired { 
        get => _expired; 
        set {
            _timer?.Dispose();
            if(!Expired)
                _timer = new Timer(_ => _expired = true, null, TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan);
        }
    }

    public string GetBearerToken(IConfiguration configuration) =>
        (_bearerToken == null || _expired == true)
            ? RefreshBearerToken(configuration)
            : _bearerToken;

    public string RefreshBearerToken(IConfiguration configuration) {
        Expired = false;
        return Convert.ToBase64String(
            Encoding.ASCII.GetBytes(
                $"{configuration["pr.net.Email"]}:{System.Environment.GetEnvironmentVariable("pr.net.Token")}"
            )
        ); 
    }

    
}