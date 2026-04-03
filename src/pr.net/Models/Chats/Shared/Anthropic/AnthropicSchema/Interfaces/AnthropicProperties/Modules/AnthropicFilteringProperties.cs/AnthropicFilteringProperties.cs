using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicFilteringProperties : IAnthropicProperties {
    
    [JsonPropertyName("isWorthReview")]
    public AnthropicBooleanType IsWorthReview { get; set; } = new AnthropicBooleanType();

    public IEnumerable<string> GetRequiredFields() => ["isWorthReview"];

}
     