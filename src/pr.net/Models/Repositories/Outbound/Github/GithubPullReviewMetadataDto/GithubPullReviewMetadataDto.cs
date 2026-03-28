using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Github;

public class GithubPullReviewCreatedMetadataDto : PullReviewCreatedMetadata {

    public GithubPullReviewCreatedMetadataDto() { }

    public GithubPullReviewCreatedMetadataDto(GithubPullReviewCreatedEventDto request) {
        this.Id = request.Hook.Id;
        this.Url = request.PullRequest?.Links?.Diff?.Href ?? string.Empty;
    }
 
    public int Id { get; set; }
    public string RepoSlug { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty; 

}