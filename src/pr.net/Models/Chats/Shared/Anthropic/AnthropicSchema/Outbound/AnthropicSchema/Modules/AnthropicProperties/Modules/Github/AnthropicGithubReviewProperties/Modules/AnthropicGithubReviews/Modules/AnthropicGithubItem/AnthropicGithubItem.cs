using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubItem : IAnthropicProperties {

    public AnthropicGithubItem() {
        this.Required = [..new AnthropicGithubItemProperties().GetRequiredFields()];
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicGithubItemProperties Properties { get; set; } = new(); 

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

    public List<string> GetRequiredFields() => ["properties"];

}