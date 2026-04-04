using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicisWorthReview {
 
    [JsonPropertyName("isWorthReview")]
    public bool? IsWorthReview { get; set; } = null;

}
     