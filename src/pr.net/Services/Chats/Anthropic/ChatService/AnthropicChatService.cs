using System.Text;
using pr.net.Services.Chat.Instructions;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Anthropic;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(IConfiguration configuration, IInstructionsService instructionsService, IChatApiClient client) : IChatService { 

    public async Task GetReviews(Dictionary<string, string> diffSections) {
        ChatProvider? provider = ValidateChatProvider(configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected service.");

        List<string> instructions = await instructionsService.GetInstructions(configuration["Chat:Provider"] ?? string.Empty);
            // ?? throw new InvalidOperationException("Could not fetch instructions.");

        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}");

        string model = configuration["Chat:Model"] 
            ?? throw new InvalidOperationException("Configuration for Chat:Model could not be found or read."); 

        string maxTokensString = configuration["Chat:MaxTokens"]
            ?? throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read.");

        if(!int.TryParse(maxTokensString, out var maxTokens))
            throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read, or is in an invalid format.");

        // instructions is a per line array so we can optionally do weird stuff to it in other places
        StringBuilder instructionsBuilder = new StringBuilder();
        foreach(var instruction in instructions)
            instructionsBuilder.AppendLine(instruction);
 
        var requestDtos = new List<AnthropicRequestDto>();
            if(diffSections == null)
                throw new InvalidOperationException($"{nameof(diffSections)} was null.");
            List<AnthropicMessageDto> messages = diffSections
                .Select(diff => new AnthropicMessageDto() { Role = "user", Content = diff.Value })
                .ToList();
            requestDtos = messages
                .Select(message => new AnthropicRequestDto() { Model = model, MaxTokens = maxTokens, Messages = messages, OutputConfig = new AnthropicOutputConfig() })
                .ToList();

        List<ChatResponse> reviews = await client.RequestReviews(tokenService, requestDtos);
    } 

}