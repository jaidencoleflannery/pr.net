using pr.net.Models;
using pr.net.Services.Tokens;
using pr.net.Services.Clients;
using pr.net.Services.Instructions;

namespace pr.net.Services.Requests;

public class RequestService {
    public async Task ProcessNewPullRequest(
        ILogger logger, 
        HttpClient httpClient, 
        IConfiguration configuration, 
        ITokenService tokenService, 
        IInstructionsService contextService, 
        NewPullRequestDto request
    ) {
        try {
            // get the pull request diff
            var pullRequestMetadata = new RequestPullReviewDto(request);
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