using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IConfiguration _configuration, IRepositoryRequestService _repositoryService, IChatService _chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) { 
        // get the pull request's diff (seperated per file).
        IEnumerable<DiffSection> diffFiles = await _repositoryService.GetPullReviewFiles(prEvent); 

        // if enabled, filter diffs for ones that are worth review.
        IEnumerable<DiffSection> filteredDiffFiles = (_configuration.GetValue<bool>("Chat:Filtering:Filter") is true)
            ? await _chatService.FilterDiffsAsync(diffFiles)
            : diffFiles;         

        // get each review object (object contains file).
        IEnumerable<(DiffSection, ChatResponse)> reviews = await _chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await _repositoryService.PostChatReviews(reviews, prEvent);
    }

}