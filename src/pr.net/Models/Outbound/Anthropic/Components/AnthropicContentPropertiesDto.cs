using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesDto {

    [JsonPropertyName("raw")]
    public AnthropicContentPropertiesItemDto Raw { get; set; } = new AnthropicContentPropertiesItemDto();

    [JsonPropertyName("markup")]
    public AnthropicContentPropertiesItemDto Markup { get; set; } = new AnthropicContentPropertiesItemDto();

} 