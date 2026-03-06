using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeNewPullRequestDto {

    [JsonPropertyName("actor")]
    public ClaudeAccountDto Account { get; set; } = new ClaudeAccountDto();


    [JsonPropertyName("pullrequest")]
    public ClaudePullRequestDto PullRequest { get; set; } = new ClaudePullRequestDto();


    [JsonPropertyName("repository")]
    public ClaudeRepositoryDto Repository { get; set; } = new ClaudeRepositoryDto();

} 