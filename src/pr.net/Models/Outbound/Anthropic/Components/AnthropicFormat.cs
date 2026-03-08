using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicFormat {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "json_schema";

    [JsonPropertyName("schema")]
    public AnthropicSchema? Scheme { get; set; } = new AnthropicSchema();

}