using pr.net.Models.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Tooling;

namespace pr.net.Services.Chat;

public interface IChatClient {

    Task<IEnumerable<DiffSection>?> RequestFilteringAsync(
        IEnumerable<DiffSection> diffSections, 
        long maxTokens, 
        string model, 
        string instructions, 
        TimeSpan? timeout
    );

    Task<List<(DiffSection, ChatResponse)>?> RequestReviewsAsync(
        IEnumerable<DiffSection> diffSections, 
        long maxTokens, 
        string model, 
        string instructions, 
        TimeSpan? timeout
    );

    Task<List<(DiffSection, ToolResponse)>?> QueryForToolUsage(
        IEnumerable<DiffSection> diffSections,
        long maxTokens,
        string model,
        string instructions,
        TimeSpan? timeout
    );
    
}
