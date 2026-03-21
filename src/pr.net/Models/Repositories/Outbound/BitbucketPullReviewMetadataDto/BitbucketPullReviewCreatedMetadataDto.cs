using pr.net.Models.Incoming.Bitbucket;

namespace pr.net.Models.Outbound.Bitbucket;

public class BitbucketPullReviewCreatedMetadataDto {

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