using System.Text.Json.Serialization;

namespace pr.net.Models.Types;

public class BooleanType {
    
    [JsonPropertyName("type")]
    public string Type { get; } = "boolean";

}
