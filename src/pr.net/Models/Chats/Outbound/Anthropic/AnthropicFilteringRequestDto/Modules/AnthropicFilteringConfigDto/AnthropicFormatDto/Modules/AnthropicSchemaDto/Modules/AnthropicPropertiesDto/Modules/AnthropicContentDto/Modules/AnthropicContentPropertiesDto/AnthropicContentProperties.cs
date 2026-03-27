using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringContentPropertiesDto {

    [JsonPropertyName("raw")]
    public AnthropicFilteringRawDto Raw { get; set; } = new AnthropicFilteringRawDto(); 

} 