using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Chat;

public interface IChatService {
    
    Task<Dictionary<string, ChatResponse>> GetChatReviewsAsync(Dictionary<string, string> diffSections);

}