using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicParentDto {

    [JsonPropertyName("parent")]
    public string? Parent { get; set; } = "object";
    
    [JsonPropertyName("properties")]
    public AnthropicParentPropertiesDto Properties { get; set; } = new AnthropicParentPropertiesDto();

    [JsonPropertyName("requied")]
    public List<string> Required { get; set; } = new List<string>() { "id" };

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

}