using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Github;

public class GithubPullReviewCreatedEventDto : PullReviewCreatedEvent {

    [JsonPropertyName("hook")]
    public GithubHookDto Hook { get; set; } = new GithubHookDto();

    [JsonPropertyName("repository")]
    public GithubRepositoryDto Repository { get; set; } = new GithubRepositoryDto();

}