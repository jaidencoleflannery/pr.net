namespace pr.net.Services.Anthropic;
public class AnthropicChatService() {
    // get review for each diff file and then request reviews
            List<ChatResponse> reviews = await chatService.RequestReviews(httpClient, configuration, tokenService, contextService, diffSections, pullRequestMetadata.Id);
            await chatService.PostReviews(httpClient, configuration, tokenService, contextService, diffSections, reviews, pullRequestMetadata);
}