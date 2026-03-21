using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicContentDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public AnthropicTextDto Text { get; set; } = new AnthropicTextDto(); 
}