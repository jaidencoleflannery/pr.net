using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicFilteringTextDto : ChatResponseText {

    [JsonPropertyName("content")]
    public AnthropicFilteringTextContentDto Content { get; set; } = new AnthropicFilteringTextContentDto();

}