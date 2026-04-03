using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicStringType {
    
    [JsonPropertyName("type")]
    public string Type { get; }= "string";

}