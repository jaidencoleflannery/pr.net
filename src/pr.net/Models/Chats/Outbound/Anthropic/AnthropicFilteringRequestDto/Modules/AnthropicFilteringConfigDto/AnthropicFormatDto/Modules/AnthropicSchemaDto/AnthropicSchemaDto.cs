using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringSchemaDto {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicFilteringPropertiesDto? Properties { get; set; } = new AnthropicFilteringPropertiesDto();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() { "content" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;
}