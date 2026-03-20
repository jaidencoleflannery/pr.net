using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicResponseContentTextContentDto {

    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;

    [JsonPropertyName("inline")]
    public AnthropicInlineDto Inline { get; set; } = new AnthropicInlineDto();
}