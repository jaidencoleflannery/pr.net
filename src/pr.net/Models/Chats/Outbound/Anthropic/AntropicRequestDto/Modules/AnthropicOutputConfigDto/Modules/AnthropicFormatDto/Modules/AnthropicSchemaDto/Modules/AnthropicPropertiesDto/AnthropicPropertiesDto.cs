using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicPropertiesDto {

    [JsonPropertyName("content")]
    public AnthropicContentDto Contents { get; set; } = new AnthropicContentDto(); 

}

