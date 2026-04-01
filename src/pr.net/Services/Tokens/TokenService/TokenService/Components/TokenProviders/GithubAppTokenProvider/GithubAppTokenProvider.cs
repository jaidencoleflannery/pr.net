using System.Text.Json;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Github;
using pr.net.Models.Tokens;

namespace pr.net.Services.Tokens;

public class GithubAppTokenProvider(HttpClient _client, IConfiguration _config) : ITokenProvider { 

    // catch query if it's for the token so we can use github's logic.
    public async ValueTask<string> FetchAsync(Token target, PullReviewCreatedEvent? prEvent = null) { 
        if(target == Token.PR_NET_REPO_TOKEN)
            return (prEvent is not null)
                ? (await FetchJwtAsync(prEvent)).Token
                : throw new InvalidOperationException($"PullReviewCreatedEvent was null, but is required for Github JWT fetching, error encountered in {nameof(FetchAsync)}");
        else
            return await FetchAsync(target); 
    }

    public ValueTask<string> FetchAsync(Token target) =>
        ValueTask.FromResult(System.Environment.GetEnvironmentVariable(Tokens.GetString(target!).ToUpper())
            ?? throw new InvalidOperationException($"{target} environment variable not found.")); 

    public async ValueTask<GithubAccessTokenResponseDto> FetchJwtAsync(PullReviewCreatedEvent prEvent) {
        string appId = _config["Repo:Github:AppId"]
            ?? throw new InvalidOperationException("Could not find Repo:Github:AppId in configuration file.");

        string appName = _config["Repo:Github:AppName"]
            ?? throw new InvalidOperationException("Could not find Repo:Github:AppName in configuration file.");

        if(prEvent is not GithubPullReviewCreatedEventDto request)
            throw new InvalidOperationException($"Event type did not match injected service type {nameof(FetchJwtAsync)}.");

        Jwt jwt = new();
        jwt.Payload.Iss = appId;
        // github has a buffer time span for tokens.
        jwt.Payload.Iat = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();
        jwt.Payload.Exp = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();

        using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/app/installations/{request.Installation.Id}/access_tokens")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.Encode());
            message.Headers.Add("User-Agent", appName);
            
            var response = await _client.SendAsync(message);  
            if(response is not null && response.Content is not null) {
                if(response.IsSuccessStatusCode)
                    return (await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>())!;
                else
                    throw new InvalidOperationException($"Github response: {response.StatusCode} - {await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>()}");
            } else {
                throw new InvalidOperationException($"Error fetching Access Token from Github in {nameof(FetchJwtAsync)}");
            }
        } 
    }

}