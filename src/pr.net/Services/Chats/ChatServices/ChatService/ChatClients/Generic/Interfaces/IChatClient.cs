using pr.net.Models.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Schemas;

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

    Task<ToolingQuery?> QueryForToolUsage(
        DiffSection diffSection,
        long maxTokens,
        string model,
        string instructions,
        TimeSpan? timeout
    );
    
}

