using System.Text.Json.Serialization;

namespace pr.net.Models.Types;

public class IntType {
    
    [JsonPropertyName("type")]
    public string Type { get; } = "integer";

}