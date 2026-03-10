using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesDto {

    [JsonPropertyName("raw")]
    public AnthropicContentPropertiesItemDto Raw { get; set; } = new AnthropicContentPropertiesItemDto();

} 