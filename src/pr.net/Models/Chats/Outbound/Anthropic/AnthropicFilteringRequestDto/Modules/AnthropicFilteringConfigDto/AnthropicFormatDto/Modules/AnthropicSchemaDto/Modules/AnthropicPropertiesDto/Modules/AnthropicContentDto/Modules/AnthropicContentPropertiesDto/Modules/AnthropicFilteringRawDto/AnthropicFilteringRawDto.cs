using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringRawDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "bool";

    [JsonPropertyName("needs_review")]
    public bool NeedsReview { get; set; } = false;  

}