using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IRepositoryRequestService requestService, IChatService chatService) {

    public async Task ProcessNewPullRequest(PullReviewCreatedEvent prEvent) {
        // get each file and it's associated diff
        Dictionary<string, string> diffFiles = await requestService.GetPullRequestFiles(prEvent);

        // get reviews for each file
        chatService.
    }

}