using System.Text.Json.Serialization;

namespace pr.net.Models;

public class NewPullRequestDto {

    [JsonPropertyName("actor")]
    public AccountDto Account { get; set; }


    [JsonPropertyName("pullrequest")]
    public PullRequest PullRequest { get; set; } 


    [JsonPropertyName("repository")]
    public RepositoryDto Repository { get; set; }

} 