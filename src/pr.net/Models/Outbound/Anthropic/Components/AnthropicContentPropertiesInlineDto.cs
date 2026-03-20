using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesInlineDto {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicContentPropertiesInlinePropertiesDto Properties { get; set; } = new AnthropicContentPropertiesInlinePropertiesDto();

    [JsonPropertyName("required")]
    public string[] Required { get; set; } = new string[] { "to" }; 

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

}