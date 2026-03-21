using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicContentPropertiesDto {

    [JsonPropertyName("raw")]
    public AnthropicRawDto Raw { get; set; } = new AnthropicRawDto() { Description = "The comment that is judging this code change." };

    [JsonPropertyName("inline")]
    public AnthropicInlineDto Inline { get; set; } = new AnthropicInlineDto();

} 