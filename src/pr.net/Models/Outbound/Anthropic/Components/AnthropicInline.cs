using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicInline {

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public int? To { get; set; } = null;

    // avoid using "from", it is for the previous version of code in the PR
    [JsonPropertyName("from")]
    public int? From { get; set; } = null;

} 