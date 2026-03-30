using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Github;

public class GithubPullReviewCreatedMetadataDto : PullReviewCreatedMetadata {

    public GithubPullReviewCreatedMetadataDto() { }

    public GithubPullReviewCreatedMetadataDto(GithubPullReviewCreatedEventDto request) {
        this.Number = request.Number;
        this.DiffUrl = request.PullRequest?.DiffUrl ?? string.Empty;
    }
 
    public long Number { get; set; } = -1;

    public string DiffUrl { get; set; } = string.Empty;

}