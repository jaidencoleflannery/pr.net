using System.Threading.Tasks;
using pr.net.Models;

namespace pr.net.Services;

public class RequestEngine {
    public async Task ProcessNewPullRequest(ILogger logger, HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, ClaudeNewPullRequestDto request) {
        var pullRequest = request.PullRequest;
        try {
            ClaudeRequestPullReviewDto content = pullRequest.ToRequestPullReviewDto(); 
            string diff = await PullRequestApiClient.GetPullReviewData(httpClient, configuration, authService, content);
            string review = await PullRequestApiClient.RequestReview(httpClient, configuration, authService, contextService, diff, content);
            Console.WriteLine(review);
        } catch (Exception exception) {
            logger.LogError(exception, $"{DateTime.Now}: Error processing pull request with Id: {pullRequest.Id} for Repository: {pullRequest.Destination.Repository.FullName}. Review not posted.");
            return;
        }
    }
}