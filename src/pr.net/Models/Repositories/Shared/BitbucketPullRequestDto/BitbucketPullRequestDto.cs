using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketPRDto {

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("author")]
    public BitbucketAccountDto Author { get; set; } = new BitbucketAccountDto();

    [JsonPropertyName("source")]
    public BitbucketSourceDto Source { get; set; } = new BitbucketSourceDto();

    [JsonPropertyName("destination")]
    public BitbucketDestinationDto Destination { get; set; } = new BitbucketDestinationDto();

    [JsonPropertyName("merge_commit")]
    public BitbucketCommitDto MergeCommit { get; set; } = new BitbucketCommitDto();

    [JsonPropertyName("participants")]
    public List<BitbucketAccountDto> Participants { get; set; } = new List<BitbucketAccountDto>();

    [JsonPropertyName("reviewers")]
    public List<BitbucketAccountDto> Reviewers { get; set; } = new List<BitbucketAccountDto>();

    [JsonPropertyName("close_source_branch")]
    public bool CloseSourceBranch { get; set; }

    [JsonPropertyName("closed_by")]
    public BitbucketAccountDto ClosedBy { get; set; } = new BitbucketAccountDto();

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("updated_on")]
    public DateTime UpdatedOn { get; set; }

    [JsonPropertyName("links")]
    public BitbucketLinksDto Links { get; set; } = new BitbucketLinksDto();

}