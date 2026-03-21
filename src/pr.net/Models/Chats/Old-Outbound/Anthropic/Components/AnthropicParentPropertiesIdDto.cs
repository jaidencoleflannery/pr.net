using System.Text.Json.Serialization;

public class AnthropicParentPropertiesIdDto {

    [JsonPropertyName("type")]
    public List<string?> Type { get; set; } = new List<string>() { "string", "null" }!;

}