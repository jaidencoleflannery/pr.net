using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContent {

    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;

    [JsonPropertyName("markup")]
    public string Markup { get; set; } = string.Empty;

} 