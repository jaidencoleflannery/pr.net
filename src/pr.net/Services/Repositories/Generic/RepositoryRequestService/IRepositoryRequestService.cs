using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Repositories.Generic;

public interface IRepositoryRequestService {
    
    Task<Dictionary<string, string>> GetPullReviewFiles();

    Task PostChatReviews(List<ChatResponseText> reviews);

}