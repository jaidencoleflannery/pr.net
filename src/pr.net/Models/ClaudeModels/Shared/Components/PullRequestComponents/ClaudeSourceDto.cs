using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeSourceDto {

    [JsonPropertyName("branch")]
    public ClaudeBranchDto Branch { get; set; } = new ClaudeBranchDto();

    [JsonPropertyName("commit")]
    public ClaudeCommitDto Commit { get; set; } = new ClaudeCommitDto();

    [JsonPropertyName("repository")]
    public ClaudeRepositoryDto Repository { get; set; } = new ClaudeRepositoryDto();

}
