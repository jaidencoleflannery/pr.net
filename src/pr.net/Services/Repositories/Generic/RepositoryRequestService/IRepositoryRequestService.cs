using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryRequestService {
    
    Task<IEnumerable<DiffSection>> GetPullReviewFiles(PullReviewCreatedEvent prEvent);

    Task PostChatReviews(List<ChatResponseText> reviews, PullReviewCreatedEvent prEvent);

}