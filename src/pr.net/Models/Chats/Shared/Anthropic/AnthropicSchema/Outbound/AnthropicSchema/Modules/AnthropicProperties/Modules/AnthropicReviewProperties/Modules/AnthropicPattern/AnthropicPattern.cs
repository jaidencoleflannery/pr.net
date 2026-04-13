using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicPattern {

    public AnthropicPattern() {
        this.Required = [..Properties.GetRequiredFields()];
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicPatternProperties Properties { get; set; } = new();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false; 

}