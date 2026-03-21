using pr.net.Models.Incoming.Bitbucket;
using pr.net.Services.Tokens;
using pr.net.Services.Instructions;
using pr.net.Services.Clients.Bitbucket;

namespace pr.net.Services.Requests.Bitbucket;

public class BitbucketRequestService {
    public async Task ProcessNewPullRequest(
        ILogger logger, 
        HttpClient httpClient, 
        IConfiguration configuration, 
        ITokenService tokenService, 
        IInstructionsService contextService, 
        PRCreatedEvent prEvent
    ) {
        try {
            // get the pull request diff
            RequestPullReviewDto pullRequestMetadata = new RequestPullReviewDto(prEvent);

            string diff = await PullRequestApiClient.GetPullRequestData(httpClient, configuration, tokenService, pullRequestMetadata);
            // split it per file
            Dictionary<string, string> diffSections = ParserService.ParseDiff(diff);
            string path = ParserService.ParsePathFromDiff(diff); 

            // get review for each diff file
            List<AnthropicResponseDto> reviews = await PullRequestApiClient.RequestReviews(httpClient, configuration, tokenService, contextService, diffSections, pullRequestMetadata.Id);

            // push reviews to pull request
            await PullRequestApiClient.PostReviews(httpClient, configuration, tokenService, contextService, path, diffSections, reviews, pullRequestMetadata);

            return;
        } catch (Exception exception) {
            logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request with Id: {request.PullRequest.Id} for Repository: {request.PullRequest.Destination.Repository.FullName}. Review not posted. ]\n");

            return;
        }
    }
}