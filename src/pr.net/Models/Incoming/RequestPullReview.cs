namespace pr.net.Models;

public class RequestPullReviewDto {
 
    public string Id { get; set; } = string.Empty;
    public string RepoSlug { get; set; } = string.Empty;
    public string? Url { get; set; } = string.Empty; // this literally gives us everything we need, we keep Id and RepoSlug just incase

}