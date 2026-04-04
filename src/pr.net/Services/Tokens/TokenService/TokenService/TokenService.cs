using System.Collections.Concurrent;

using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

// a token factory that tracks token expiration.
public class TokenService(ITokenProvider _provider) : ITokenService {

    private ConcurrentDictionary<Token, ICachedToken> _tokens = new ConcurrentDictionary<Token, ICachedToken>();
    private static readonly SemaphoreSlim _repoLock = new SemaphoreSlim(1, 1);

    // ensure token exists and is not expired
    public async ValueTask<string> GetTokenAsync(Token type, PullReviewCreatedEvent prEvent) {   
        if(!_tokens.TryGetValue(type, out var token)) { 
            await _repoLock.WaitAsync(); // avoid multiple refreshes on init
            try {
                if(_tokens.TryGetValue(type, out var gateToken)) // force each thread to individually check this as queue empties
                    return await gateToken!.GetValueAsync();
                var newToken = await CachedToken.Initialize(_provider, type, prEvent);
                _tokens.TryAdd(type, newToken!);
                return await newToken!.GetValueAsync();
            } finally {
                _repoLock.Release();
            }
        } else if(token.Expired == true) {
            await _repoLock.WaitAsync(); 
            try {
                if(token.Expired == false) // same logic as above
                    return await token.GetValueAsync();
                else
                    return await (await token.RefreshAsync(prEvent)).GetValueAsync();
            } finally {
                _repoLock.Release();
            }
        }
        return await token!.GetValueAsync();
    }
    
}