using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicResponseContentDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public AnthropicResponseContentTextDto Text { get; set; } = new AnthropicResponseContentTextDto(); 
}