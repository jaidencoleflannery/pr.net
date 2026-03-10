using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesItemDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

}