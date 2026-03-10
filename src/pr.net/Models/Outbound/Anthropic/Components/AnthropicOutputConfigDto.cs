using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicOutputConfig : OutputConfig {

    [JsonPropertyName("format")]
    public AnthropicFormatDto? Format { get; set; } = new AnthropicFormatDto();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() { "content", "inline" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;
    
}