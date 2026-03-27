using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringPropertiesDto {

    [JsonPropertyName("content")]
    public AnthropicFilteringContentDto Contents { get; set; } = new AnthropicFilteringContentDto(); 


}

