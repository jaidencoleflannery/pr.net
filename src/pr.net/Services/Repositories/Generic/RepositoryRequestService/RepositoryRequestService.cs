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
            if(ParserService.ParseDiff(diff, out List<DiffSection> diffSections))
                return diffSections;
            else
                throw new InvalidOperationException($"Failure parsing diffs from format in {nameof(GetPullReviewFiles)}.");
        } catch (Exception exception) {
            _logger.LogError($"\n{DateTime.Now}: {exception}\n[ Error processing pull request. Review not posted. ]\n");
            throw;
        }
    }

    // posts reviews to specific pull review
    public async Task PostChatReviews(IEnumerable<(DiffSection, ChatResponse)> reviews, PullReviewCreatedEvent prEvent) {  
        var result = await _client.PostReviewsAsync(reviews, prEvent);
    }

}