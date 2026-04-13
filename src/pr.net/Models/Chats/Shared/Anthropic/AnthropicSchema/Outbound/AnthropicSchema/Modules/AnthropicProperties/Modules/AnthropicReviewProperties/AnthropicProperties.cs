using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicReviewProperties : IAnthropicProperties {

    [JsonPropertyName("reviews")]
    public AnthropicReviews Reviews { get; set; } = new(); 

    public List<string> GetRequiredFields() => ["reviews"];

}