using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonUsage {

    [JsonPropertyName("inputTokens")]
    public int InputTokens { get; set; } = -1;

    [JsonPropertyName("outputTokens")]
    public int OutputTokens { get; set; } = -1;

    [JsonPropertyName("totalTokens")]
    public int TotalTokens { get; set; } = -1;

    [JsonPropertyName("cacheReadInputTokens")]
    public int CacheReadInputTokens { get; set; } = -1;

    [JsonPropertyName("cacheWriteInputTokens")]
    public int CacheWriteInputTokens { get; set; } = -1;

}
