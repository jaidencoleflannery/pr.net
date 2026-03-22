using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;

namespace pr.net.Services.Repositories.Generic;

public interface IApiClient {

    Task<string> GetPullRequestData(ITokenService tokenService, PullReviewCreatedMetadata request);

    Task<List<string>> PostReviews(ITokenService tokenService, List<ChatResponseText> reviews, PullReviewCreatedMetadata request);

}