using System.Collections.Concurrent;

using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

// a token factory that tracks token expiration.
public class TokenService( 
    IRepoTokenHandler _repoTokenHandler, 
    IChatTokenHandler _chatTokenHandler,
    ILogger<TokenService> _logger
) : ITokenService {

    private ConcurrentDictionary<Token, ICachedToken> _tokens = new ConcurrentDictionary<Token, ICachedToken>();
    private static readonly SemaphoreSlim _repoLock = new SemaphoreSlim(1, 1);

    // ensure token exists and is not expired.
    public async ValueTask<string?> GetTokenAsync(Token type, PullReviewCreatedEvent prEvent) {   
        if(!_tokens.TryGetValue(type, out var token)) { 
            await _repoLock.WaitAsync(); // avoid multiple refreshes on init.
            try {
                if(_tokens.TryGetValue(type, out var gateToken)) // force each thread to individually check this as queue empties.
                    return await gateToken!.GetValueAsync();
                ICachedToken? newToken = null;
                // switch map - this layering exists so the token can be processed for it's specific target.
                switch(type) {
                    case Token.PR_NET_REPO_TOKEN:
                        newToken = await CachedToken.Initialize(_repoTokenHandler, type, prEvent); 
                        break;

                    case Token.PR_NET_CHAT_TOKEN:
                        newToken = await CachedToken.Initialize(_chatTokenHandler, type, prEvent); 
                        break;
                }
                if(newToken == null) {
                    _logger.LogError($"\n{DateTime.Now}: [ Error fetching token of type: {type}, in {nameof(GetTokenAsync)}. ]\n");
                    return null;
                }

                _tokens.TryAdd(type, newToken!);
                return await newToken!.GetValueAsync();
            } finally {
                _repoLock.Release();
            }
        } else if(token.Expired == true) {
            await _repoLock.WaitAsync(); 
            try {
                if(token.Expired == false) // same logic as above.
                    return await token.GetValueAsync();
                else
                    // we do not need to give the provider again, this process is a singleton so it already has it cached from the initial call.
                    return await (await token.RefreshAsync(prEvent)).GetValueAsync();
            } finally {
                _repoLock.Release();
            }
        }
        return await token!.GetValueAsync();
    }
    
}