using pr.net.Models.Tokens;

namespace pr.net.Services.Tokens;

public class GithubAppTokenProvider(HttpClient _client) : EnvTokenProvider, ITokenProvider { 

    // catch query if its for the token
    public async override ValueTask<string> FetchAsync(Token target) { 
        if(target == Token.PR_NET_REPO_TOKEN)
            return await FetchJwtAsync();
        return await base.FetchAsync(target); 
    }

    public async ValueTask<string> FetchJwtAsync() {
        string appIdString = Environment.GetEnvironmentVariable("Repo:Github:AppId")
            ?? throw new InvalidOperationException("Could not find Repo:Github:AppId in configuration file.");

        Jwt jwt = new();
        jwt.Payload.Iss = appIdString;
        // github has a buffer timeset for tokens
        jwt.Payload.Iat = DateTimeOffset.UtcNow.AddSeconds(-60).ToUnixTimeSeconds();
        jwt.Payload.Exp = DateTimeOffset.UtcNow.AddSeconds(10).ToUnixTimeSeconds();

        using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/app/installations/{appIdString}/access_tokens")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.Encode());
            
            var response = await _client.SendAsync(message); 
            var content = await response.Content.ReadAsStringAsync();
            if(response.IsSuccessStatusCode)
                return content; 
            else
                throw new InvalidOperationException($"Github response: {response.StatusCode} - {content}");
        } 
    }

}