using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class ChatTokenHandler(
    ITokenProvider _tokenProvider
) : IChatTokenHandler { 
    public async ValueTask<string?> FetchAsync(PullReviewCreatedEvent? prEvent = null) {
        string? token = await _tokenProvider.FetchAsync(Token.PR_NET_CHAT_TOKEN, prEvent);
        if(string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException($"Could not fetch chat token from storage in {nameof(FetchAsync)}.");
        return token;
    }

}