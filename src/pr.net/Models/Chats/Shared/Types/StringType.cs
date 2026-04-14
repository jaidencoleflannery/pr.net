using System.Text.Json.Serialization;

namespace pr.net.Models.Types;

public class StringType {
    
    [JsonPropertyName("type")]
    public string Type { get; } = "string";

}