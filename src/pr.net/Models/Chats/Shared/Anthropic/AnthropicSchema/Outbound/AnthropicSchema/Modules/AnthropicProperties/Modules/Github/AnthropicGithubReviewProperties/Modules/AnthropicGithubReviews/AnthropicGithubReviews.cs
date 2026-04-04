using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubReviews : IAnthropicProperties {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "array";

    [JsonPropertyName("items")]
    public AnthropicGithubItem Items { get; set; } = new();  

    public List<string> GetRequiredFields() => ["items"];

}