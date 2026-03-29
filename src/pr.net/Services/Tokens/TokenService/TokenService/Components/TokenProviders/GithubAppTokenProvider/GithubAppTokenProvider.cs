namespace pr.net.Services.Tokens;

public class GithubAppTokenProvider(HttpClient _client) : ITokenProvider { 

    public ValueTask<string> FetchAsync(string target) {
        
    }

}