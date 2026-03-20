using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicInlineDto {

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int To { get; set; } = 0;

} 