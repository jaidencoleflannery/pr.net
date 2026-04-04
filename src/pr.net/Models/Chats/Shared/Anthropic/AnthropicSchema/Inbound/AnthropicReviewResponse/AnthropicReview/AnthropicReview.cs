using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicReview {
 
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("line")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Line { get; set; } = 0;

}