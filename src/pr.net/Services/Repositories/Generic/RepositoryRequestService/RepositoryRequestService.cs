using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Parsing;

namespace pr.net.Services.Requests;

public class RepositoryRequestService(ILogger<RepositoryRequestService> _logger, IRepositoryApiClient _client) : IRepositoryRequestService {

    // returns a dictionary of key: file, value: diff
    public async Task<Dictionary<string, string>> GetPullReviewFiles() {
        try {
            // get the pull request diff
            string diff = await _client.GetPullRequestDataAsync();

            // split diff per file, diffSections should be key: file, value: diff
            Dictionary<string, string> diffSections = ParserService.ParseDiff(diff); 
            return diffSections;
        } catch (Exception exception) {
            _logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request. Review not posted. ]\n");
            throw new Exception("Failed to pull and parse diff.");
        }
    }

    // posts reviews to specific pull review
    public async Task PostChatReviews(List<ChatResponseText> reviews) {  
        var result = await _client.PostReviewsAsync(reviews);
    }

}