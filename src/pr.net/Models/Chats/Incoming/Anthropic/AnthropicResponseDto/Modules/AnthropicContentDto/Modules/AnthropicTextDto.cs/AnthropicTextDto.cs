using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicTextDto : ChatResponseText {

    [JsonPropertyName("content")]
    public AnthropicTextContentDto Content { get; set; } = new AnthropicTextContentDto();
}