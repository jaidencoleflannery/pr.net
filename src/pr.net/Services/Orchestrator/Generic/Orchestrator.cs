using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IRepositoryRequestService repoService, IChatService chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) {
        // get each file's associated diff
        Dictionary<string, string> diffFiles = await repoService.GetPullReviewFiles(prEvent);

        // get each file's associated review
        Dictionary<string, ChatResponse> reviews = await chatService.GetChatReviewsAsync(diffFiles);

        // post reviews to branch
        //await repoService.PostChatReviews();
    }

}