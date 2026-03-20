using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicResponseContentTextDto {

    [JsonPropertyName("content")]
    public AnthropicResponseContentTextContentDto Content { get; set; } = new AnthropicResponseContentTextContentDto();
}