using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class RepoTokenHandler(
    ITokenProvider _tokenProvider
) : IRepoTokenHandler { 
    public async ValueTask<string?> FetchAsync(PullReviewCreatedEvent? prEvent = null) {
        string? token = await _tokenProvider.FetchAsync(Token.PR_NET_REPO_TOKEN, prEvent);
        if(string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException($"Could not fetch repository token from storage in {nameof(FetchAsync)}.");
        return token;
    }

}