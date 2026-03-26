using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Chat;

public interface IChatService {

    Task<Dictionary<string, string>> FilterDiffsAsync(Dictionary<string, string> diffSections);
    
    Task<List<ChatResponseText>> GetChatReviewsAsync(Dictionary<string, string> filteredDiffSections); 

}