using pr.net.Services.Repositories.Generic;
using pr.net.Services.Parsing;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Requests;

public class RepositoryRequestService(ILogger<RepositoryRequestService> _logger, IRepositoryApiClient _client) : IRepositoryRequestService {

    // returns a dictionary of key: file, value: diff.
    public async Task<IEnumerable<DiffSection>> GetPullReviewFiles(PullReviewCreatedEvent prEvent) {
        try {
            // get the pull request diff.
            string diff = await _client.GetPullRequestDataAsync(prEvent);

            // split diff per file, diffSections should be key: file, value: diff.
            IEnumerable<DiffSection> diffSections = ParserService.ParseDiff(diff); 
            return diffSections;
        } catch (Exception exception) {
            _logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request. Review not posted. ]\n");
            throw new Exception("Failed to pull and parse diff.");
        }
    }

    // posts reviews to specific pull review
    public async Task PostChatReviews(List<ChatResponseText> reviews, PullReviewCreatedEvent prEvent) {  
        var result = await _client.PostReviewsAsync(reviews, prEvent);
    }

}