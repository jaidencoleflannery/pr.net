using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonServiceTier {

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

}
