using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonContent {

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}