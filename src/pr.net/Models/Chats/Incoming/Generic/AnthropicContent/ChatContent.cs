using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming;

public class ChatContent {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("line")]
    public int Line { get; set; } = -1;
}