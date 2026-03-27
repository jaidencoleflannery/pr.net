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
        
        if(filteredDiffFiles.Values.Count <= 0) {
            Console.WriteLine("\n\u28FF\u28D3\u28FF\u2895\u28FF\u28FF\u28FF\u2833 [ No diffs were worth review. Returning early. ] \u28B7\u28FF\u2833\u28FF\u28FF\u28D3\u2895\u28FF\n");
            return;
        }

        // get each review object (contains file)
        List<ChatResponseText> reviews = await chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await repoService.PostChatReviews(reviews, prEvent);
    }

}