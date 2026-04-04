using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubFilteringProperties : IAnthropicProperties {

    const string propertyName = "isWorthReview";
    
    [JsonPropertyName(propertyName)]
    public AnthropicBooleanType IsWorthReview { get; set; } = new();

    public IEnumerable<string> GetRequiredFields() => [propertyName];

}
     