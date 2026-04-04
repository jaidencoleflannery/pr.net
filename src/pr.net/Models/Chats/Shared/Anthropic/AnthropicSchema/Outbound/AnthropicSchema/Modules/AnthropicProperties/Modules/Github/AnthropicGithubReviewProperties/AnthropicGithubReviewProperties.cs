using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubReviewProperties : IAnthropicProperties {

    [JsonPropertyName("body")]
    public List<AnthropicStringType> Body { get; set; } = [];

    [JsonPropertyName("line")]
    public int Line { get; set; } = 2;

    public IEnumerable<string> GetRequiredFields() => ["body", "line"];

}