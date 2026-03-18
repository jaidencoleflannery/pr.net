using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesRawDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

}