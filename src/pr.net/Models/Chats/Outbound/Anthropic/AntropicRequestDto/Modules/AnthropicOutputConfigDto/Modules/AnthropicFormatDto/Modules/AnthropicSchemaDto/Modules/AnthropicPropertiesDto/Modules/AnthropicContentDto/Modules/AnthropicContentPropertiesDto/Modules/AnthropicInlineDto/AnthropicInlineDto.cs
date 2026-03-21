using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicInlineDto {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicContentPropertiesInlinePropertiesDto Properties { get; set; } = new AnthropicContentPropertiesInlinePropertiesDto();

    [JsonPropertyName("required")]
    public string[] Required { get; set; } = new string[] { "to" }; 

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

}