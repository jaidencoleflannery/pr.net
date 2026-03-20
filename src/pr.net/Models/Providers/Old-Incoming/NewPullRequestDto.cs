using System.Text.Json.Serialization;
using pr.net.Models.Incoming;

namespace pr.net.Models;

public class NewPullRequestDto {

    [JsonPropertyName("actor")]
    public AccountDto Account { get; set; } = new AccountDto();

    [JsonPropertyName("pullrequest")]
    public PullRequestDto PullRequest { get; set; } = new PullRequestDto();

    [JsonPropertyName("repository")]
    public RepositoryDto Repository { get; set; } = new RepositoryDto();

} 