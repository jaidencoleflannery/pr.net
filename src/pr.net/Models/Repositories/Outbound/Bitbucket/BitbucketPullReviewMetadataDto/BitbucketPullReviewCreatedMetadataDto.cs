using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Bitbucket;

public class BitbucketPullReviewCreatedMetadataDto : PullReviewCreatedMetadata {

    public BitbucketPullReviewCreatedMetadataDto() { }

    public BitbucketPullReviewCreatedMetadataDto(BitbucketPullReviewCreatedEventDto request) {
        this.Id = request.PullRequest.Id;
        this.RepoSlug = request.Repository.FullName;
        this.Url = request.PullRequest?.Links?.Diff?.Href ?? string.Empty;
    }
 
    public int Id { get; set; }
    public string RepoSlug { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty; 

}