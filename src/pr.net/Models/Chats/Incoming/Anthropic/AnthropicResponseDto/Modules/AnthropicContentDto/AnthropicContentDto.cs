using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicContent {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty; // Anthropic's response will be a string in the structure of our output config, nested modules should imitate it.
}