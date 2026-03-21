using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicInlinePropertiesDto {

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public int? To { get; set; } = null;
    
}