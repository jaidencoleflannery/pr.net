using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Chat;

public interface IChatService {
    
    Task<List<ChatResponseText>> GetChatReviewsAsync(Dictionary<string, string> diffSections);

}