using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicReviewProperties : IAnthropicProperties {

    const string propertyName = "review";
    
    [JsonPropertyName(propertyName)]
    public AnthropicStringType Review { get; set; } = new();

    public IEnumerable<string> GetRequiredFields() => [propertyName];

}