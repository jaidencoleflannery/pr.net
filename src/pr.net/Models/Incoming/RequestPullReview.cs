using pr.net.Models.Incoming;

namespace pr.net.Models;

public class RequestPullReviewDto {

    public RequestPullReviewDto() { }

    public RequestPullReviewDto(NewPullRequestDto request) {
        this.Id = request.PullRequest.Id;
        this.RepoSlug = request.Repository.FullName;
        this.Url = request.PullRequest?.Links?.Diff?.Href ?? string.Empty;
    }
 
    public int Id { get; set; }
    public string RepoSlug { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty; // this literally gives us everything we need, we keep Id and RepoSlug just incase

}