using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicInlineDto {

    [JsonPropertyName("path")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Path { get; set; } = null;

    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? To { get; set; } = null;

} 