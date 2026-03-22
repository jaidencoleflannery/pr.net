using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketSourceDto {

    [JsonPropertyName("branch")]
    public BitbucketBranchDto Branch { get; set; } = new BitbucketBranchDto();

    [JsonPropertyName("commit")]
    public BitbucketCommitDto Commit { get; set; } = new BitbucketCommitDto();

    [JsonPropertyName("repository")]
    public BitbucketRepositoryDto Repository { get; set; } = new BitbucketRepositoryDto();

}

