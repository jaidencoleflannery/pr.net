using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicSchemaDto {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicPropertiesDto? Properties { get; set; } = new AnthropicPropertiesDto();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() { "content", "inline" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;
}