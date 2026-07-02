using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicResponse : ChatResponse {

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty; 
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;  

    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; } = string.Empty;

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; set; } = null;

    [JsonPropertyName("usage")]
    public AnthropicUsage Usage { get; set; } = new AnthropicUsage();

}

