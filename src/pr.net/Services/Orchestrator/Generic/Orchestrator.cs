using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IRepositoryRequestService repoService, IChatService chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) { 
        // get each file's associated diff
        Dictionary<string, string> diffFiles = await repoService.GetPullReviewFiles(prEvent);

        // filter diffs for ones that are worth review
        Dictionary<string, string> filteredDiffFiles = await chatService.FilterDiffsAsync(diffFiles);

        // get each review object (contains file)
        List<ChatResponseText> reviews = await chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await repoService.PostChatReviews(reviews, prEvent);
    }

}