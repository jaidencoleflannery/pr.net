using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicFilteringTextContentDto {

    [JsonPropertyName("raw")]
    public bool Raw { get; set; } = false;
 
}