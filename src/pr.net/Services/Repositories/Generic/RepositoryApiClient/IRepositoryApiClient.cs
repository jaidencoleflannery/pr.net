using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryApiClient {

    Task<string> GetPullRequestDataAsync();

    Task<List<string>> PostReviewsAsync(List<ChatResponseText> reviews);

}