using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicBooleanType {
    
    [JsonPropertyName("type")]
    public string Type { get; } = "boolean";

}