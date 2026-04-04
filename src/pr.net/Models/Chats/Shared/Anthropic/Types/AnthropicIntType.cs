using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicIntType {
    
    [JsonPropertyName("type")]
    public string Type { get; } = "integer";

}