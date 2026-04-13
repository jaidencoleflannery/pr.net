using System.Text.Json;

using Anthropic.Models.Messages;

using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Chat;

public interface IChatClient {

    Task<IEnumerable<DiffSection>?> RequestFilteringAsync(IEnumerable<DiffSection> diffSections, long maxTokens, string model, string instructions, TimeSpan? timeout);

    Task<List<(DiffSection, ChatResponse)>?> RequestReviewsAsync(IEnumerable<DiffSection> diffSections, long maxTokens, string model, string instructions, TimeSpan? timeout);
    
}