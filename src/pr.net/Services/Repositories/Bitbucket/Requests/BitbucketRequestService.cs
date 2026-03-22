using pr.net.Models.Incoming.Bitbucket;
using pr.net.Models.Outbound.Bitbucket;
using pr.net.Services.Clients.Bitbucket; 
using pr.net.Services.Tokens;
using pr.net.Services.Repositories.Generic;

namespace pr.net.Services.Requests.Bitbucket;

public class BitbucketRequestService {

    private BitbucketApiClient _client;

    public BitbucketRequestService(IHttpClientFactory factory) {
        _client = new(factory); // spin up the api client and inject the factory
    }

    // returns a dictionary of key: file, value: diff
    public async Task<Dictionary<string, string>> GetPullRequestFiles(ILogger logger, IApiClient httpClient, ITokenService tokenService, BitbucketPullReviewCreatedEventDto prEvent) {
        try {
            // get the pull request diff
            BitbucketPullReviewCreatedMetadataDto pullRequestMetadata = new(prEvent); // grab necesarry metadata (optimize to just cast directly on receival?)
            string diff = await _client.GetPullRequestData(tokenService, pullRequestMetadata);

            // split diff per file, diffSections should be key: file, value: diff
            Dictionary<string, string> diffSections = BitbucketParserService.ParseDiff(diff); 
            return diffSections;
        } catch (Exception exception) {
            logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request with Id: {prEvent.PullRequest.Id}. Review not posted. ]\n");
            throw new Exception("Failed to pull and parse diff.");
        }
    }

}