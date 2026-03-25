using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryApiClient {

    Task<string> GetPullRequestData(PullReviewCreatedMetadata request);

    Task<List<string>> PostReviews(Dictionary<string, ChatResponseText> reviews, PullReviewCreatedMetadata request);

}