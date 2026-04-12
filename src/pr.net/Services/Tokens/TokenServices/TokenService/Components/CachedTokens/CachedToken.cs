using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class CachedToken : ICachedToken {  
    private ITokenHandler? _tokenHandler;
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

    public static async ValueTask<ICachedToken?> Initialize(ITokenHandler handler, Token type, PullReviewCreatedEvent? prEvent = null) {
        CachedToken instance = new CachedToken();
        instance._tokenHandler = handler;
        instance._type = type;
        if(instance._tokenHandler == null)
            return null;
        return await instance.RefreshAsync(prEvent);
    } 

    public async ValueTask<string?> GetValueAsync() =>
        await ValueTask.FromResult(_token!);

    public async ValueTask<ICachedToken?> RefreshAsync(PullReviewCreatedEvent? prEvent = null) {
        if(_tokenHandler == null)
            throw new InvalidOperationException($"No Token Handler was provided in {RefreshAsync}.");

        _token = await _tokenHandler!.FetchAsync(prEvent);
        if(_token == null) {
            throw new InvalidOperationException("Provider failed to fetch token, value null."); 
        } else {
            Expired = false;
            return this;
        }
    }

}