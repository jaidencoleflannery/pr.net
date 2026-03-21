using pr.net.Models.Incoming.Bitbucket;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Bitbucket;
using pr.net.Services.Clients.Bitbucket; 
using pr.net.Services.Tokens;
using pr.net.Services.Instructions;

namespace pr.net.Services.Requests.Bitbucket;

public class BitbucketRequestService {
    public async Task ProcessNewPullRequest(
        ILogger logger, 
        HttpClient httpClient, 
        IConfiguration configuration, 
        ITokenService tokenService, 
        IInstructionsService contextService, 
        IChatService chatService,
        BitbucketPullReviewCreatedEventDto prEvent
    ) {
        try {
            // get the pull request diff
            BitbucketPullReviewCreatedMetadataDto pullRequestMetadata = new(prEvent); // grab necesarry metadata (optimize to just cast directly on receival?)
            string diff = await BitbucketApiClient.GetPullRequestData(httpClient, tokenService, pullRequestMetadata);

            // split diff per file, diffSections should be key: file, value: diff
            Dictionary<string, string> diffSections = BitbucketParserService.ParseDiff(diff);

            // get review for each diff file and then request reviews
            List<ChatResponse> reviews = await chatService.RequestReviews(httpClient, configuration, tokenService, contextService, diffSections, pullRequestMetadata.Id);
            await chatService.PostReviews(httpClient, configuration, tokenService, contextService, diffSections, reviews, pullRequestMetadata);

            return;
        } catch (Exception exception) {
            logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request with Id: {prEvent.PullRequest.Id}. Review not posted. ]\n");

            return;
        }
    }
}