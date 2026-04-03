using pr.net.Models.Generic;
using pr.net.Models.Incoming.Anthropic;

namespace pr.net.Services.Chat;

public interface IChatService {

    Task<IEnumerable<DiffSection>> FilterDiffsAsync(IList<DiffSection> diffSections);
    
    Task<List<(DiffSection, ChatResponse)>> GetChatReviewsAsync(List<DiffSection> diffSections); 

}