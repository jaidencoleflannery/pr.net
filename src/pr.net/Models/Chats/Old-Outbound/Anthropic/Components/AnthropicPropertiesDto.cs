using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicPropertiesDto : PropertiesDto {

    [JsonPropertyName("content")]
    public AnthropicContentDto Contents { get; set; } = new AnthropicContentDto(); 

}
