using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicItems : IAnthropicProperties {

    public AnthropicItems() {
        this.Required = [..new AnthropicItemProperties().GetRequiredFields()];
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicItemProperties Properties { get; set; } = new(); 

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

    public List<string> GetRequiredFields() => ["properties"];

}