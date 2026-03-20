using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicContentPropertiesDto? Properties { get; set; } = new AnthropicContentPropertiesDto(); 

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new List<string>() { "raw", "to" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

}