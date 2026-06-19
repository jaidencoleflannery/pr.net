using pr.net.Services.Repositories.Generic;
using pr.net.Services.Parsing;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Generic;

namespace pr.net.Services.Requests;

public class RepositoryRequestService(
        ILogger<RepositoryRequestService> _logger, 
        IRepositoryApiClient _client
    ) : IRepositoryRequestService {

    // returns a dictionary of key: file, value: diff.
    public async Task<IEnumerable<DiffSection>?> GetPullReviewFiles(PullReviewCreatedEvent prEvent) {
        try {
            // get the pull request diff.
            string? diff = await _client.GetPullRequestDataAsync(prEvent);
            if(diff == null) {
                _logger.LogError($"\n{DateTime.Now}: [ Error, fiff files could not be fetched. in {nameof(GetPullReviewFiles)}]\n");
                return null;
            }

            // split diff per file, diffSections should be key: file, value: diff.
            if(ParserService.ParseDiff(diff, out List<DiffSection> diffSections))
                return diffSections; 
            else
                return null;
        } catch (Exception exception) {
            _logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request. Diff files could not be fetched. ]\n");
            return null;
        }
    }

    // posts reviews to specific pull review
    public async Task PostChatReviews(IEnumerable<(DiffSection, ChatResponse)> reviews, PullReviewCreatedEvent prEvent) {  
        List<string?> result = await _client.PostReviewsAsync(reviews, prEvent);
        _logger.LogInformation($"\n{DateTime.Now}: Posted total of ({result.Count}) reviews..\n");
        return;
    }

}
