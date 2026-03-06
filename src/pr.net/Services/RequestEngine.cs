using System.Threading.Tasks;
using pr.net.Models;

namespace pr.net.Services;

public class RequestEngine {
    public async Task ProcessNewPullRequest(ILogger logger, HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, PullRequestDto request) {
        try {
            RequestPullReviewDto content = request.ToRequestPullReviewDto(); 
            string diff = await PullRequestApiClient.GetPullReviewData(httpClient, configuration, authService, content);
            string review = await PullRequestApiClient.RequestReview(httpClient, configuration, diff, contextService);
            return;
        } catch (Exception exception) {
            logger.LogError(exception, $"{DateTime.Now}: Error processing pull request with Id: {request.Id} for Repository: {request.Destination.Repository.FullName}. Review not posted.");
            return;
        }
    }
}