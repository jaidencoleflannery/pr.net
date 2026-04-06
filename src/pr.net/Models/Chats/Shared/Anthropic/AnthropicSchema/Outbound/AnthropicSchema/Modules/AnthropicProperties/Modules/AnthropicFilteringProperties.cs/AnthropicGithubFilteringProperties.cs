using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubFilteringProperties : IAnthropicProperties {
 
    [JsonPropertyName("isWorthReview")]
    public AnthropicBooleanType IsWorthReview { get; set; } = new();

    public List<string> GetRequiredFields() => ["isWorthReview"];

}
     