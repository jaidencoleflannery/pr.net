using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicUsageDto {

    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; } = -1;

    [JsonPropertyName("cache_creation_input_tokens")]
    public int CacheCreationInputTokens { get; set; } = -1;

    [JsonPropertyName("cache_read_input_tokens")]
    public int CacheReadInputTokens { get; set; } = -1;

    [JsonPropertyName("cache_creation")]
    public AnthropicCacheCreationDto CacheCreation { get; set; } = new AnthropicCacheCreationDto();

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; } = -1; 

    [JsonPropertyName("service_tier")]
    public string ServiceTier { get; set; } = string.Empty;

    [JsonPropertyName("inference_geo")]
    public string InferenceGeo { get; set; } = string.Empty;
    
}