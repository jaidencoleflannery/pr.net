using System.Text.Json.Serialization;

namespace pr.net.Models.Types;

public class NullableIntType {
    
    [JsonPropertyName("type")]
    public string[] Type { get; } = ["integer", "null"];

}
