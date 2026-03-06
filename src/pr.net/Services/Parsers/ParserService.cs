using pr.net.Models;

namespace pr.net.Services;

public static class ParserService {
    public static ClaudeRequestPullReviewDto ToRequestPullReviewDto(this ClaudePullRequestDto request) {
        return new ClaudeRequestPullReviewDto {
            Id = request.Id,
            RepoSlug = request.Destination.Repository.FullName,
            Url = request.Links.Diff!.Href ?? string.Empty
        }; 
    }

}