using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicReviewResponse {
 
    [JsonPropertyName("review")]
    public List<AnthropicReview>? Reviews { get; set; } = new();

} 