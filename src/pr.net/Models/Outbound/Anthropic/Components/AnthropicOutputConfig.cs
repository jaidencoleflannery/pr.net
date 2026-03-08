using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicOutputConfig {

    [JsonPropertyName("format")]
    public AnthropicFormat? Format { get; set; } = new AnthropicFormat();
    
}