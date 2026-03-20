using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Bitbucket;

public class BitbucketPRCreatedDto {

    [JsonPropertyName("actor")]
    public BitbucketAccountDto Account { get; set; } = new BitbucketAccountDto();

    [JsonPropertyName("pullrequest")]
    public BitbucketPRDto PullRequest { get; set; } = new BitbucketPRDto();

    [JsonPropertyName("repository")]
    public BitbucketRepositoryDto Repository { get; set; } = new BitbucketRepositoryDto();

}