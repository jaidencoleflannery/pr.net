namespace pr.net.Services.Tokens;

public class CachedToken(ITokenProvider provider, Token token) : ICachedToken { 
    private readonly Object _lock = new();
    private string _token = string.Empty; 
    private bool _tokenExpired = true;
    private System.Threading.Timer? _tokenTimer;
    private bool TokenExpired { 
        get => _tokenExpired; 
        set {
            _tokenTimer?.Dispose();
            if(!value) {
                _tokenExpired = value;
                _tokenTimer = new Timer(_ => _tokenExpired = true, null, TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan);
            }
        }
    } 

    public ValueTask<string> GetAsync() {
        lock(_lock) {
            if(!string.IsNullOrWhiteSpace(_token) && !_tokenExpired) {
                return ValueTask.FromResult(
                    _token
                );
            }
            else {
                TokenExpired = false;
                return ValueTask.FromResult(
                    _token = provider.FetchAsync(token.ToString()).GetAwaiter().GetResult()
                ); 
            }
        }
    }

}