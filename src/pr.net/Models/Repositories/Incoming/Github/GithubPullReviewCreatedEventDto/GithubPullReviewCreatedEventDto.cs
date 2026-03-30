using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Github;

public class GithubPullReviewCreatedEventDto : PullReviewCreatedEvent {

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public long Number { get; set; } = -1;

    [JsonPropertyName("pull_request")]
    public GithubPullRequestDto PullRequest { get; set; } = new();

    [JsonPropertyName("repository")]
    public GithubRepositoryDto Repository { get; set; } = new();

    [JsonPropertyName("installation")]
    public GithubInstallationDto Installation { get; set; } = new();

}