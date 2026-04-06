using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicReviews : IAnthropicProperties {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "array";

    [JsonPropertyName("items")]
    public AnthropicItems Items { get; set; } = new();  

    public List<string> GetRequiredFields() => ["items"];

}