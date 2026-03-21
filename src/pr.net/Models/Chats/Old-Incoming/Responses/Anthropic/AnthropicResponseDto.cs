using System.Text.Json.Serialization;
using pr.net.Models.Incoming;

namespace pr.net.Models;

public class AnthropicResponseDto {

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty; 
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty; 

    [JsonPropertyName("content")]
    public List<AnthropicResponseContentDto> Content { get; set; } = new List<AnthropicResponseContentDto>();

    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; } = string.Empty;

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; set; } = null;

    [JsonPropertyName("usage")]
    public AnthropicUsageDto Usage { get; set; } = new AnthropicUsageDto();
}