using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicOutputConfig : OutputConfig {

    [JsonPropertyName("format")]
    public AnthropicFormatDto? Format { get; set; } = new AnthropicFormatDto();
    
}