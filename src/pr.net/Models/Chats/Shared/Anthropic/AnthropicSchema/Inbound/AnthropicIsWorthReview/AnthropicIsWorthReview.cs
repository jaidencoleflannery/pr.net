using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicIsWorthReview {
 
    [JsonPropertyName("isWorthReview")]
    public bool? IsWorthReview { get; set; } = null;

}
     