using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicPropertiesDto : PropertiesDto {

    [JsonPropertyName("content")]
    public AnthropicContentDto? Content { get; set; } = new AnthropicContentDto(); 

    [JsonPropertyName("inline")]
    public AnthropicInlineDto? Inline { get; set; } = new AnthropicInlineDto();

}
