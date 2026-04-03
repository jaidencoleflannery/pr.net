using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Chat;

public interface IChatService {

    Task<IEnumerable<DiffSection>> FilterDiffsAsync(IEnumerable<DiffSection> diffSections);
    
    Task<IEnumerable<(DiffSection, ChatResponse)>> GetChatReviewsAsync(IEnumerable<DiffSection> diffSections);

}