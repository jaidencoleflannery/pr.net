using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicSchemaDto {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicPropertiesDto? Properties { get; set; } = new AnthropicPropertiesDto();

    [JsonPropertyName("required")]
    public string[] Required = [];

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;
}