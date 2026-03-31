using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryApiClient {

    Task<string> GetPullRequestDataAsync(PullReviewCreatedEvent prEvent);

    Task<List<string>> PostReviewsAsync(List<ChatResponseText> reviews, PullReviewCreatedEvent prEvent);

}