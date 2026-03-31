using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryRequestService {
    
    Task<Dictionary<string, string>> GetPullReviewFiles(PullReviewCreatedEvent prEvent);

    Task PostChatReviews(List<ChatResponseText> reviews, PullReviewCreatedEvent prEvent);

}