using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(
    IConfiguration _configuration, 
    IRepositoryRequestService _repositoryService, 
    IChatService _chatService
) {
    // each function is expected to handle logging and return null - don't handle errors at this scope.
    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent, string userId) {  

        // get each file's associated diff.
        IEnumerable<DiffSection>? diffFiles = await _repositoryService.GetPullReviewFiles(prEvent); 
        if(diffFiles == null)
            return;

        // if enabled, filter diffs for ones that are worth review.
        IEnumerable<DiffSection>? filteredDiffFiles = (_configuration.GetValue<bool>("Chat:Filtering:Filter") is true)
            ? await _chatService.FilterDiffsAsync(diffFiles, userId)
            : diffFiles;
        if(filteredDiffFiles == null)
            return;

        // get each review object (object contains file).
        IEnumerable<(DiffSection, ChatResponse)>? reviews = await _chatService.GetChatReviewsAsync(filteredDiffFiles, userId);
        if(reviews == null)
            return;

        // post reviews to branch
        await _repositoryService.PostChatReviews(reviews, prEvent);
    }

}