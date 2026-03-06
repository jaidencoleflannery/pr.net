using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeCommitDto {

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

}