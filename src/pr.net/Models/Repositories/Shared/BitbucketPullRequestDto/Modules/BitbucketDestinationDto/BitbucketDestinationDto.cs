using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketDestinationDto {

    [JsonPropertyName("branch")]
    public BitbucketBranchDto Branch { get; set; } = new BitbucketBranchDto();

    [JsonPropertyName("commit")]
    public BitbucketCommitDto Commit { get; set; } = new BitbucketCommitDto();

}