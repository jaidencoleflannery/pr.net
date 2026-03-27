using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringContentDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicFilteringContentPropertiesDto? Properties { get; set; } = new AnthropicFilteringContentPropertiesDto(); 

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() { "raw" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

}