using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicResponseContentDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string? Text { get; set; } = string.Empty; 


    [JsonPropertyName("inline")]
    public AnthropicInlineDto? Inline { get; set; } = null;

}