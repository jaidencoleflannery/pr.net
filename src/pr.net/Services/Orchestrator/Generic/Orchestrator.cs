using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IConfiguration configuration, IRepositoryRequestService repoService, IChatService chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) { 
        // get each file's associated diff
        Dictionary<string, string> diffFiles = await repoService.GetPullReviewFiles(prEvent);

        // if enabled, filter diffs for ones that are worth review
        Dictionary<string, string> filteredDiffFiles = (configuration.GetValue<bool>("Chat:Filtering:Filter") == true)
            ? await chatService.FilterDiffsAsync(diffFiles)
            : diffFiles;

        // get each review object (contains file)
        List<ChatResponseText> reviews = await chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await repoService.PostChatReviews(reviews, prEvent);
    }

}