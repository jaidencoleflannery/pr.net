using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryApiClient {

    Task<string> GetPullRequestData(PullReviewCreatedEvent request);

    Task<List<string>> PostReviews(List<ChatResponseText> reviews, PullReviewCreatedEvent request);

}