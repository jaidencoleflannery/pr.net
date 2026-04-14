using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryApiClient {

    Task<string?> GetPullRequestDataAsync(PullReviewCreatedEvent prEvent);

    Task<List<string?>> PostReviewsAsync(IEnumerable<(DiffSection, ChatResponse)> reviews, PullReviewCreatedEvent prEvent);

}