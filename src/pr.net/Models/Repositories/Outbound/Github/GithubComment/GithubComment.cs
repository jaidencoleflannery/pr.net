using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubComment {

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("commit_id")]
    public long? CommitId { get; set; } = null;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("start_line")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StartLine { get; set; } = null;

    [JsonPropertyName("start_side")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StartSide { get; set; } = null;

    [JsonPropertyName("line")]
    public int Line { get; set; } = -1;

    [JsonPropertyName("side")]
    public string Side { get; set; } = "RIGHT";

}