using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Services.Context;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Orchestration;

public class Orchestrator(IConfiguration _configuration, IRepositoryRequestService _repositoryService, IChatService _chatService) {

    public async Task ProcessNewPullRequest() { 
        // get each file's associated diff.
        Dictionary<string, string> diffFiles = await _repositoryService.GetPullReviewFiles();

        // if enabled, filter diffs for ones that are worth review.
        Dictionary<string, string> filteredDiffFiles = (_configuration.GetValue<bool>("Chat:Filtering:Filter") is true)
            ? await _chatService.FilterDiffsAsync(diffFiles)
            : diffFiles;
        
        if(filteredDiffFiles.Values.Count <= 0) {
            Console.WriteLine("\nNo diffs were deemed worthy of review.\n");
            return;
        }

        // get each review object (contains file)
        List<ChatResponseText> reviews = await _chatService.GetChatReviewsAsync(filteredDiffFiles);

        // post reviews to branch
        await _repositoryService.PostChatReviews(reviews);
    }

}