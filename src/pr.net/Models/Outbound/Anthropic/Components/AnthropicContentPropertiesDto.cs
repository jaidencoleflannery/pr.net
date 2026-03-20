using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesDto {

    [JsonPropertyName("raw")]
    public AnthropicContentPropertiesRawDto Raw { get; set; } = new AnthropicContentPropertiesRawDto() { Description = "The comment that is judging this code change." };

    [JsonPropertyName("inline")]
    public AnthropicContentPropertiesInlineDto Inline { get; set; } = new AnthropicContentPropertiesInlineDto();

} 