using pr.net.Models;

namespace pr.net.Services;

public static class ParserService {
    public static RequestPullReviewDto ToRequestPullReviewDto(this PullRequestDto request) {
        return new RequestPullReviewDto {
            Id = request.Id,
            RepoSlug = request.Destination.Repository.FullName,
            Url = request.Links.Diff!.Href ?? string.Empty
        }; 
    }

}