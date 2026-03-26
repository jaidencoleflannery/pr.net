using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicTextContentDto {

    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;
 
}