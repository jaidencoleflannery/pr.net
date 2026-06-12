using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Bitbucket;

public class BitbucketPullReviewCreatedMetadataDto : PullReviewCreatedMetadata {
    public int Id { get; set; }
    public string CommitHash { get; set; } = string.Empty;
    public string RepoSlug { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;  

    public BitbucketPullReviewCreatedMetadataDto() { }

    public BitbucketPullReviewCreatedMetadataDto(BitbucketPullReviewCreatedEventDto request) {
        this.Id = request.PullRequest.Id;
        this.CommitHash = request?.PullRequest?.MergeCommit?.Hash ?? string.Empty;
        this.RepoSlug = request.Repository.FullName;
        this.Url = request.PullRequest?.Links?.Diff?.Href ?? string.Empty;
    } 
}

