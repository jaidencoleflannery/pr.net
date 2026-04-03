using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IConfiguration _configuration, IRepositoryRequestService _repositoryService, IChatService _chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) { 
        // get each file's associated diff.
        List<DiffSection> diffFiles = [..await _repositoryService.GetPullReviewFiles(prEvent)];
        if(diffFiles.Count <= 0) {
            Console.WriteLine("\nFetch was successful, but no diffs were found.\n");
            return;
        }

        // if enabled, filter diffs for ones that are worth review.
        List<DiffSection> filteredDiffFiles = (_configuration.GetValue<bool>("Chat:Filtering:Filter") is true)
            ? [..await _chatService.FilterDiffsAsync(diffFiles)]
            : diffFiles; 
        if(filteredDiffFiles.Count <= 0) {
            Console.WriteLine("\nNo diffs were deemed worthy of review.\n");
            return;
        }

        // get each review object (object contains file).
        List<(DiffSection, ChatResponse)> reviews = await _chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await _repositoryService.PostChatReviews(reviews, prEvent);
    }

}