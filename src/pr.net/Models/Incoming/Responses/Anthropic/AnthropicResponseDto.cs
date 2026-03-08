using System.Text.Json.Serialization;
using pr.net.Models.Incoming;

namespace pr.net.Models;

public class AnthropicResponseDto {
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<AnthropicContentDto> Content { get; set; } = new List<AnthropicContentDto>();

    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; } = string.Empty;

    [JsonPropertyName("stop_sequence")]
    public string StopSequence { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public AnthropicUsageDto Usage { get; set; } = new AnthropicUsageDto();
}