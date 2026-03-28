using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Parsing;

namespace pr.net.Services.Requests;

public class RepositoryRequestService(ILogger<RepositoryRequestService> logger, IRepositoryApiClient client) : IRepositoryRequestService {

    // returns a dictionary of key: file, value: diff
    public async Task<Dictionary<string, string>> GetPullReviewFiles(PullReviewCreatedEvent prEvent) {
        try {
            // get the pull request diff
            string diff = await client.GetPullRequestData(prEvent);

            // split diff per file, diffSections should be key: file, value: diff
            Dictionary<string, string> diffSections = ParserService.ParseDiff(diff); 
            return diffSections;
        } catch (Exception exception) {
            logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request. Review not posted. ]\n");
            throw new Exception("Failed to pull and parse diff.");
        }
    }

    // posts reviews to specific pull review
    public async Task PostChatReviews(List<ChatResponseText> reviews, PullReviewCreatedEvent prEvent) {  
        var result = await client.PostReviews(reviews, prEvent);
    }

}