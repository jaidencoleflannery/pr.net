using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicTextDto {

    [JsonPropertyName("content")]
    public AnthropicTextContentDto Content { get; set; } = new AnthropicTextContentDto();
}