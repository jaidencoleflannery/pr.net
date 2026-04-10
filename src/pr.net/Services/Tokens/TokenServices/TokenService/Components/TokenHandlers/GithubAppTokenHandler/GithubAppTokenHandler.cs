using Microsoft.Extensions.Options;

using pr.net.Configurations.Repo;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Github;
using pr.net.Models.Tokens;

namespace pr.net.Services.Tokens;

public class GithubAppTokenHandler(
    HttpClient _client, 
    IOptions<RepoConfiguration> _options,
    ILogger<GithubAppTokenHandler> _logger,
    ITokenProvider _tokenProvider
) : IRepoTokenHandler { 
    private RepoConfiguration _configuration = _options.Value;

    public async ValueTask<string?> FetchAsync(PullReviewCreatedEvent? prEvent = null) =>
        (await this.FetchJwtAsync(prEvent!))?.Token ?? null; 

    private async ValueTask<GithubAccessTokenResponseDto?> FetchJwtAsync(PullReviewCreatedEvent prEvent) {
        if(prEvent is not GithubPullReviewCreatedEventDto request)
            throw new InvalidOperationException($"Event type did not match injected service type {nameof(FetchJwtAsync)}.");

        string? appId = _configuration?.Github?.AppId ?? null;
        if(string.IsNullOrWhiteSpace(appId))
            throw new InvalidOperationException($"[ Could not fetch Github AppId from configuration in {nameof(FetchJwtAsync)}. ]\n");

        string? appName = _configuration?.Github?.AppName ?? null;
        if(string.IsNullOrWhiteSpace(appName)) {
            _logger.LogError($"\n{DateTime.Now}: [ Could not fetch Github AppName from configuration in {nameof(FetchJwtAsync)}. ]\n");
            return null;
        }

        string? token = await _tokenProvider.FetchAsync(Token.PR_NET_REPO_TOKEN, prEvent);
        if(string.IsNullOrWhiteSpace(token)) {
            _logger.LogError($"\n{DateTime.Now}: [ Could not fetch repository token from storage for JWT encoding in {nameof(FetchJwtAsync)}. ]\n");
            return null;
        }

        Jwt jwt = new();
        jwt.Payload.Iss = appId;
        // github has a buffer time span for tokens.
        jwt.Payload.Iat = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();
        jwt.Payload.Exp = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();

        using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/app/installations/{request.Installation.Id}/access_tokens")) {
            string? jwtToken = jwt.Encode(token);
            if(string.IsNullOrWhiteSpace(jwtToken)) {
                _logger.LogError($"\n{DateTime.Now}: [ Could not encode JWT secret in {nameof(FetchJwtAsync)}. ]\n");
                return null;
            }

            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.Encode(jwtToken));
            message.Headers.Add("User-Agent", appName);
            
            var response = await _client.SendAsync(message);  
            if(response is not null && response.Content is not null) {
                if(response.IsSuccessStatusCode)
                    return (await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>())!;
                _logger.LogError($"\n{DateTime.Now}: [ Failed to fetch JWT, Github response: {response.StatusCode} - {await response.Content.ReadFromJsonAsync<GithubAccessTokenResponseDto>()},\n in {nameof(FetchJwtAsync)}. ]\n");
                return null;
            } else {
                _logger.LogError($"\n{DateTime.Now}: [ Unexpected error fetching Access Token from Github in {nameof(FetchJwtAsync)}. ]\n");
                return null;
            }
        } 
    }

}