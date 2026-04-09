using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class CachedToken : ICachedToken {  
    private ITokenProvider? _provider;
    private Token _type;  
    private string? _token; 
    private bool _tokenExpired = true;
    private System.Threading.Timer? _tokenTimer;
    public bool Expired { 
        get => _tokenExpired; 
        private set {
            _tokenTimer?.Dispose();
            if(!value) {
                _tokenExpired = value;
                _tokenTimer = new Timer(_ => _tokenExpired = true, null, TimeSpan.FromMinutes(10), Timeout.InfiniteTimeSpan);
            }
        }
    }

    private CachedToken() { }

    public static async ValueTask<ICachedToken> Initialize(ITokenProvider provider, Token type, PullReviewCreatedEvent prEvent) {
        CachedToken instance = new CachedToken();
        instance._provider = provider;
        instance._type = type;
        if(instance._provider == null)
            throw new InvalidOperationException($"Failed to set provider or type for {type} cached token.");
        return await instance.RefreshAsync(prEvent);
    } 

    public async ValueTask<string> GetValueAsync() =>
        await ValueTask.FromResult(_token!);

    public async ValueTask<ICachedToken> RefreshAsync(PullReviewCreatedEvent prEvent) {
        if(_provider == null)
            throw new InvalidOperationException("Cached token has no provider or type set."); 

        _token = await _provider!.FetchAsync(_type, prEvent);
        if(_token == null) {
            throw new InvalidOperationException("Provider failed to fetch token, value null."); 
        } else {
            Expired = false;
            return this;
        }
    }

}