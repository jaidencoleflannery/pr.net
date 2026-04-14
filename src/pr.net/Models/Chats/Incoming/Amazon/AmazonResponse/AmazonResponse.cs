using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonResponse : ChatResponse {
    
    [JsonPropertyName("stopReason")]
    public string StopReason { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public AmazonUsage Usage { get; set; } = new();

    [JsonPropertyName("metrics")]
    public AmazonMetrics Metrics { get; set; } = new();

    [JsonPropertyName("performance_config")]
    public AmazonPerformanceConfig PerformanceConfig { get; set; } = new();

    [JsonPropertyName("serviceTier")]
    public AmazonServiceTier ServiceTier { get; set; } = new();

}