using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class WebhookSecretHandler(
    ITokenProvider _tokenProvider
) : IRepoTokenHandler { 
    public async ValueTask<string?> FetchAsync(PullReviewCreatedEvent? prEvent = null) {
        string? token = await _tokenProvider.FetchAsync(Token.PR_NET_REPO_TOKEN, prEvent);
        if(string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException($"Could not fetch webhook secret from storage in {nameof(FetchAsync)}.");
        return token;
    }

}