using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonMetrics {

    [JsonPropertyName("latencyMs")]
    public int LatencyMs { get; set; } = -1;

}