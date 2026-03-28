using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Github;

public class GithubPullReviewCreatedEventDto : PullReviewCreatedEvent {

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public int Number { get; set; } = -1;

    [JsonPropertyName("pull_request")]
    public GithubPullRequestDto PullRequest { get; set; } = new GithubPullRequestDto();

    [JsonPropertyName("repository")]
    public GithubRepositoryDto Repository { get; set; } = new GithubRepositoryDto();

}