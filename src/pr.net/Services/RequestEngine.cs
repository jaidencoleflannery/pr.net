using pr.net.Models;

namespace pr.net.Services;

public class RequestEngine {
    public void ProcessNewPullRequest(ILogger logger, HttpClient httpClient, IConfiguration configuration, AuthService authService, PullRequestDto request) {
        try {
            var content = request.ToRequestPullReviewDto(); 
            var response = PullRequestApiClient.GetPullReviewData(httpClient, configuration, authService, content);
            var review = PullRequestApiClient.RequestReview(httpClient, configuration, response);
            return;
        } catch (Exception exception) {
            logger.LogError(exception, $"{DateTime.Now}: Error processing pull request with Id: {request.Id} for Repository: {request.Destination.Repository.FullName}. Review not posted.");
            return;
        }
    }
}