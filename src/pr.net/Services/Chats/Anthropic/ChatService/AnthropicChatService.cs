using System.Text;
using pr.net.Services.Chat.Instructions;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Anthropic;
using pr.net.Services.Chat.Generic;
using pr.net.Models.Incoming.Anthropic;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(IConfiguration configuration, IInstructionsService instructionsService, IChatApiClient client) : IChatService { 

    public async Task<Dictionary<string, string>> FilterDiffsAsync(Dictionary<string, string> diffSections) { 
        if(diffSections.Count < 1)
            throw new InvalidOperationException($"No diffs provided to {nameof(FilterDiffsAsync)}");
        
        ChatProvider? provider = ValidateChatProvider(configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected service.");

        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}");

        string? model = null; 
        if(configuration.GetValue<bool>("Chat:Filtering:UseEmbedding"))
            Console.WriteLine("Embedding has not been configured.");
            // configure provider specific embedding
        else if(!configuration.GetValue<bool>("Chat:Filtering:UseEmbedding"))
            model = configuration["Chat:Filtering:Model"]
                ?? throw new InvalidOperationException("Configuration for Chat:Filtering:Model could not be found or read.");

        string maxTokensString = configuration["Chat:Filtering:MaxTokens"]
            ?? throw new InvalidOperationException("Configuration for Chat:Filtering:MaxTokens could not be found or read.");
        if(!int.TryParse(maxTokensString, out var maxTokens))
            throw new InvalidOperationException("Configuration for Chat:Filtering:MaxTokens could not be found or read, or is in an invalid format."); 

        string instructions = string.Join(' ', await instructionsService.GetInstructions(isForFiltering: true))
            ?? throw new InvalidOperationException("Could not fetch filtering instructions.");

        var requestsPerPath = diffSections.ToDictionary(
            diff => diff.Key,
            diff => new AnthropicRequestDto {
                Model = model!,
                MaxTokens = maxTokens,
                Messages = [new AnthropicMessageDto { Role = "user", Content = $"Is this diff worth reviewing? ```{diff.Value}```" }],
                OutputConfig = new AnthropicFilteringOutputConfigDto(),
                System = instructions
            }); 

        List<ChatResponseText> filterResponses = await client.RequestFilteringAsync(requestsPerPath.Values.ToList(), url);
        var keysToRemove = diffSections
            .Keys.Zip(filterResponses)
            .Where(pair => pair.Second is AnthropicFilteringTextDto dto && !dto.Content.Raw)
            .Select(pair => pair.First)
            .ToList();

        foreach(var key in keysToRemove)
            diffSections.Remove(key);

        return diffSections;
    }

    public async Task<List<ChatResponseText>> GetChatReviewsAsync(Dictionary<string, string> diffSections) {
        if(diffSections.Count < 1)
            throw new InvalidOperationException($"No diffs provided to {nameof(GetChatReviewsAsync)}");

        ChatProvider? provider = ValidateChatProvider(configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected service."); 

        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}");

        string model = configuration["Chat:Model"] 
            ?? throw new InvalidOperationException("Configuration for Chat:Model could not be found or read."); 

        string maxTokensString = configuration["Chat:MaxTokens"]
            ?? throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read.");
        if(!int.TryParse(maxTokensString, out var maxTokens))
            throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read, or is in an invalid format.");

        string instructions = string.Join(' ', await instructionsService.GetInstructions(isForFiltering: false))
            ?? throw new InvalidOperationException("Could not fetch filtering instructions.");
 
        var requestsPerPath = diffSections.ToDictionary(
            diff => diff.Key,
            diff => new AnthropicRequestDto {
                Model = model,
                MaxTokens = maxTokens,
                Messages = [new AnthropicMessageDto { Role = "user", Content = diff.Value }],
                OutputConfig = new AnthropicOutputConfig(),
                System = instructions
            });

        List<ChatResponseText> responses = await client.RequestReviewsAsync(requestsPerPath.Values.ToList(), url);
        foreach(var (path, response) in requestsPerPath.Keys.Zip(responses)) {
            ((AnthropicTextDto)response).Inline.Path = path;
        }
        return responses;
    } 

}