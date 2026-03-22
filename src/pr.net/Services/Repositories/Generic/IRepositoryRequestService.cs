using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryRequestService {
    
    Task<Dictionary<string, string>> GetPullRequestFiles(PullReviewCreatedEvent prEvent);

}