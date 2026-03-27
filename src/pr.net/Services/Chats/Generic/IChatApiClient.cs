using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;
using pr.net.Models.Outbound.Anthropic;

namespace pr.net.Services.Chat.Generic;

public interface IChatApiClient {

    Task<List<ChatResponseText>> RequestReviewsAsync(List<AnthropicRequestDto> requestDtos, string url);

    Task<List<ChatResponseText>> RequestFilteringAsync(List<AnthropicRequestDto> requestDtos, string url);

}