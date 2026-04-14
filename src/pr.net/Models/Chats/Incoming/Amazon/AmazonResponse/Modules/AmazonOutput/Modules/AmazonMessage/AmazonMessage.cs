using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonMessage {

    [JsonPropertyName("output")]
    public AmazonContent Message { get; set; } = new();  

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

}