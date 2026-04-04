using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicGithubReviewProperties : IAnthropicProperties {

    [JsonPropertyName("reviews")]
    public AnthropicGithubReviews Reviews { get; set; } = new(); 

    public List<string> GetRequiredFields() => ["reviews"];

}