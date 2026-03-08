using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming;

public class AnthropicUsageDto {

    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; } = -1;

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; } = -1; 
    
}