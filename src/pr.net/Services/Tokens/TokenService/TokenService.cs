namespace pr.net.Services.Tokens;

public class TokenService(ITokenProvider provider) : ITokenService {

    private Dictionary<Token, ICachedToken> _tokens = new Dictionary<string, ICachedToken>();

    ValueTask<string> GetRepoTokenAsync() {
       if(_tokens.TryGetValue(Token.PR_NET_REPO_TOKEN, out var token)) {
           return GetString(token); 
        }; 
    }

    ValueTask<string> GetChatTokenAsync() {
        
    }
    
}