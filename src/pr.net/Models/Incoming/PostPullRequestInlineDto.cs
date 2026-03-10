using System.Text.Json.Serialization;

namespace pr.net.Models;

public class PostPullRequestInlineDto {

    [JsonPropertyName("to")]
    public int? To { get; set; } = null;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

}