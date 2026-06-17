using pr.net.Services.Repositories.Generic;
using pr.net.Services.Chat;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(
    IConfiguration _configuration, 
    IRepositoryRequestService _repositoryService,
    IChatService _chatService
) {
    // each function is expected to handle errors and logging - don't handle at this scope.
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

        // run tools for context.
        IEnumerable<DiffSection>? context = await _chatService.GetChatContextAsync(filteredDiffFiles, userId);
        if(context == null)
            return;

        // get each review object (object contains file).
        IEnumerable<(DiffSection, ChatResponse)>? reviews = await _chatService.GetChatReviewsAsync(filteredDiffFiles, userId);
        if(reviews == null)
            return;

        // post reviews to branch
        await _repositoryService.PostChatReviews(reviews, prEvent);
    }
}

