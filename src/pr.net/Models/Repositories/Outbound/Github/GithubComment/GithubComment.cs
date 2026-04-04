using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubComment {

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("commit_id")]
    public string CommitId { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("start_line")]
    public int? StartLine { get; set; } = 1;

    [JsonPropertyName("start_side")]
    public string StartSide { get; set; } = "RIGHT";

    [JsonPropertyName("line")]
    public int Line { get; set; } = 2;

    [JsonPropertyName("side")]
    public string Side { get; set; } = "RIGHT";

}