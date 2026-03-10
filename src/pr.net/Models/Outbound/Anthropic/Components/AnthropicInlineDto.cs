using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicInlineDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicInlinePropertiesDto Properties { get; set; } = new AnthropicInlinePropertiesDto();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() {"path", "to", "from"};

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties = false;
 
} 