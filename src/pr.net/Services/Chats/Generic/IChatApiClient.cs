using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;

namespace pr.net.Services.Chat.Generic;

public interface IChatApiClient {

    Task<List<ChatResponse>> RequestReviewsAsync(ITokenService authService, int requestId);

    Task<List<string>> PostReviewsAsync(ITokenService tokenService, List<ChatResponseText> reviews, PullReviewCreatedMetadata request);

}