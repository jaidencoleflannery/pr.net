using pr.net.Models;

namespace pr.net.Services;

public class RequestEngine {
    public bool ProcessNewPullRequest(ILogger logger, PullRequestDto request) {
        try {
            var content = request.ToRequestPullReviewDto(); 
            var response = PullRequestApiClient.GetPullReviewData(content);
            return true;
        } catch (Exception exception) {
            Console.WriteLine()
            return false;
        }
    }
}