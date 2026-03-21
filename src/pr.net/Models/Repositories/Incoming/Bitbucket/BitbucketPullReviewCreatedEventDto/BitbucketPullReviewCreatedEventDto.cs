using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Bitbucket;

public class BitbucketPullReviewCreatedEventDto {

    [JsonPropertyName("pullrequest")]
    public BitbucketPRDto PullRequest { get; set; } = new BitbucketPRDto();

    [JsonPropertyName("repository")]
    public BitbucketRepositoryDto Repository { get; set; } = new BitbucketRepositoryDto();

}

/*
public RequestPullReviewDto(NewPullRequestDto request) {
        this.Id = request.PullRequest.Id;
        this.RepoSlug = request.Repository.FullName;
        this.Url = request.PullRequest?.Links?.Diff?.Href ?? string.Empty;
    }
*/