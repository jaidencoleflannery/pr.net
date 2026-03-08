using System.Threading.Tasks;
using pr.net.Models;

namespace pr.net.Services;

public class RequestEngine {
    public async Task ProcessNewPullRequest(ILogger logger, HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, NewPullRequestDto request) {
        try {
            // get the pull request diff and split it per file
            var pullRequestMetadata = new RequestPullReviewDto(request);
            string diff = await PullRequestApiClient.GetPullRequestData(httpClient, configuration, authService, pullRequestMetadata);
            List<string> diffSections = ParserService.ParseDiff(diff);

            // get review for each diff file
            List<PropertiesDto> reviews = await PullRequestApiClient.RequestReviews(httpClient, configuration, authService, contextService, diffSections, pullRequestMetadata.Id);

            // push reviews to pull request
            await PullRequestApiClient.PostReviews(httpClient, configuration, authService, contextService, reviews, pullRequestMetadata.Id);

        } catch (Exception exception) {
            logger.LogError($"{DateTime.Now}: {exception}", $"[ Error processing pull request with Id: {request.PullRequest.Id} for Repository: {request.PullRequest.Destination.Repository.FullName}. Review not posted. ]");
            return;
        }
    }
}