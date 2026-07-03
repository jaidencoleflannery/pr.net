using System.Text.Json.Serialization;

namespace pr.net.Models.Types;

public class NullableStringType {
    
    [JsonPropertyName("type")]
    public string[] Type { get; } = ["string", "null"];

}
