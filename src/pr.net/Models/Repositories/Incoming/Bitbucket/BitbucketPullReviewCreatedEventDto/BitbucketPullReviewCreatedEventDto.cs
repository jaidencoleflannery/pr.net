using System.Text.Json.Serialization;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Bitbucket;

public class BitbucketPullReviewCreatedEventDto : PullReviewCreatedEvent {

    [JsonPropertyName("pullrequest")]
    public BitbucketPRDto PullRequest { get; set; } = new BitbucketPRDto();

    [JsonPropertyName("repository")]
    public BitbucketRepositoryDto Repository { get; set; } = new BitbucketRepositoryDto();

}