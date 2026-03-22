using pr.net.Models.Incoming.Bitbucket;
using pr.net.Models.Outbound.Bitbucket;
using pr.net.Services.Clients.Bitbucket; 
using pr.net.Services.Tokens;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Requests.Bitbucket;

public class BitbucketRequestService(ILogger logger, ITokenService tokenService, IRepositoryApiClient client) : IRepositoryRequestService {

    // returns a dictionary of key: file, value: diff
    public async Task<Dictionary<string, string>> GetPullRequestFiles(PullReviewCreatedEvent prEvent) {
        BitbucketPullReviewCreatedEventDto createdEvent = (BitbucketPullReviewCreatedEventDto)prEvent;
        try {
            // get the pull request diff
            BitbucketPullReviewCreatedMetadataDto pullRequestMetadata = new(createdEvent); // grab necesarry metadata
            string diff = await client.GetPullRequestData(tokenService, pullRequestMetadata);

            // split diff per file, diffSections should be key: file, value: diff
            Dictionary<string, string> diffSections = BitbucketParserService.ParseDiff(diff); 
            return diffSections;
        } catch (Exception exception) {
            logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request with Id: {createdEvent.PullRequest.Id}. Review not posted. ]\n");
            throw new Exception("Failed to pull and parse diff.");
        }
    }

}