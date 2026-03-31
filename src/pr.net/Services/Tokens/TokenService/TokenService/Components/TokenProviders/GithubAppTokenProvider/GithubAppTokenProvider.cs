using System.Text.Json;

using pr.net.Services.Context;

using pr.net.Models.Github;
using pr.net.Models.Tokens;

namespace pr.net.Services.Tokens;

public class GithubAppTokenProvider(HttpClient _client, IConfiguration _config, IAmbientContextService<GithubPullReviewCreatedEventDto> _context) : EnvTokenProvider, ITokenProvider { 

    // catch query if its for the token
    public async override ValueTask<string> FetchAsync(Token target) { 
        if(target == Token.PR_NET_REPO_TOKEN)
            return await FetchJwtAsync();
        return await base.FetchAsync(target); 
    }

    public async ValueTask<string> FetchJwtAsync() {
        string appId = _config["Repo:Github:AppId"]
            ?? throw new InvalidOperationException("Could not find Repo:Github:AppId in configuration file.");

        string appName = _config["Repo:Github:AppName"]
            ?? throw new InvalidOperationException("Could not find Repo:Github:AppName in configuration file.");

        long installationId = _context.CreatedEvent.Installation.Id;

        Jwt jwt = new();
        jwt.Payload.Iss = appId;
        // github has a buffer timeset for tokens
        jwt.Payload.Iat = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();
        jwt.Payload.Exp = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();

        using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/app/installations/{installationId}/access_tokens")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.Encode());
            message.Headers.Add("User-Agent", appName);
            
            var response = await _client.SendAsync(message);  
            if(response.IsSuccessStatusCode)
                return JsonSerializer.Serialize<GithubAccessTokenResponseDto>(await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>()!);
            else
                throw new InvalidOperationException($"Github response: {response.StatusCode} - {await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>()}");
        } 
    }

}