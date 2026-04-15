using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonPerformanceConfig {

    [JsonPropertyName("latency")]
    public string Latency { get; set; } = string.Empty;

}
