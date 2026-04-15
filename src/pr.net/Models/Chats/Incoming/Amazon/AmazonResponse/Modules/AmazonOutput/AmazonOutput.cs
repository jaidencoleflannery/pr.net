using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Amazon;

public class AmazonOutput {

    [JsonPropertyName("output")]
    public AmazonMessage Message { get; set; } = new();  

}