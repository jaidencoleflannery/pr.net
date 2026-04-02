using System.Text;

using Microsoft.Extensions.AI;

using Anthropic;

using Anthropic.Exceptions;
using Anthropic.Models.Messages;

using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;

using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Anthropic;
using pr.net.Models.Incoming.Anthropic;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(IConfiguration _configuration, IInstructionsService _instructionsService, IAnthropicClient _client, ITokenService _tokenService) : IChatService { 

    public async Task<List<DiffSection>> FilterDiffsAsync(List<DiffSection> diffSections) { 
        if(diffSections.Count < 1)
            throw new InvalidOperationException($"No diffs provided to {nameof(FilterDiffsAsync)}.");
        
        ChatProvider? provider = ValidateChatProvider(_configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected Anthropic service.");

        if(_configuration.GetValue<bool>("Chat:Filtering:UseEmbedding"))
            Console.WriteLine("Embedding has not been configured."); // configure provider specific embedding service here.

        string? model = _configuration["Chat:Filtering:Model"];
        if(string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("Configuration for Chat:Filtering:Model could not be found or read.");

        if(!int.TryParse(_configuration["Chat:Filtering:MaxTokens"], out int maxTokens))
            throw new InvalidOperationException("Configuration for Chat:Filtering:MaxTokens could not be found or read, or is in an invalid format.");  

        string instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: true))
            ?? throw new InvalidOperationException("Could not fetch filtering instructions.");  

        // requestsperpath's key == file name.
        Dictionary<DiffSection, MessageCreateParams> requestsPerPath = diffSections.ToDictionary(
            diff => diff,
            diff => new MessageCreateParams {
                MaxTokens = maxTokens,
                Messages = [
                    new() {
                        Role = Role.User,
                        Content = $"Is this diff worth reviewing?\n```{diff.Contents}```"
                    },
                ],
                Model = model,
                OutputConfig = new OutputConfig {
                    Format = new JsonOutputFormat { Schema = AnthropicSchema.FilterRequestSchema }, 
                },
                Temperature = 0.0,
            }); 

        return await this.RequestFilteringAsync(requestsPerPath);
    }

    private async Task<List<DiffSection>> RequestFilteringAsync(Dictionary<DiffSection, MessageCreateParams> requestsPerPath) {
        if(!TimeSpan.TryParse(_configuration["Chat:Filtering:Timeout"], out TimeSpan timeout))
            throw new InvalidOperationException("Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format."); 

        // iterate over every instance of requestDtos and send them individually.
        var responses = new List<DiffSection>();
        var exceptions = new List<Exception>();
        foreach((DiffSection section, MessageCreateParams request) in requestsPerPath) {
            if(request.Messages.Count < 1) {
                requestsPerPath.Remove(section);
                continue;
            }

            Message message;
            try {
                message = await _client.Messages.Create(request);
                Console.WriteLine(message);
            } catch(AnthropicApiException exception) {
                Console.WriteLine($"Anthropic call failed: {exception.Message}");
                exceptions.Add(exception);
            }    
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }

    private async Task<Dictionary<String, ChatMessage>> RequestReviewsAsync(Dictionary<string, MessageCreateParams> ) { 
        // iterate over every instance of requestDtos and send them individually.
        var responses = new List<ChatMessage>();
        var exceptions = new List<Exception>();

        foreach(MessageCreateParams parameter in parameters.Values) {
            if(parameter.Messages.Count < 1)
                continue;

            Message message;
            try {
                message = await _client.Messages.Create(parameter);

                Console.WriteLine(message);
            } catch(AnthropicApiException exception) {
                Console.WriteLine($"Anthropic call failed: {exception.Message}");
                exceptions.Add(exception);
            }    
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }

    public async Task<Dictionary<string, (string, ChatMessage)>> GetChatReviewsAsync(Dictionary<string, string> diffSections) {
        if(diffSections.Count < 1)
            throw new InvalidOperationException($"No diffs provided to {nameof(GetChatReviewsAsync)}");

        ChatProvider? provider = ValidateChatProvider(_configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected service."); 
 
        string model = _configuration["Chat:Model"] 
            ?? throw new InvalidOperationException("Configuration for Chat:Model could not be found or read."); 

        string maxTokensString = _configuration["Chat:MaxTokens"]
            ?? throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read.");
        if(!int.TryParse(maxTokensString, out var maxTokens))
            throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read, or is in an invalid format.");

        string instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: false))
            ?? throw new InvalidOperationException("Could not fetch filtering instructions.");
 
        Dictionary<string, MessageCreateParams> requestsPerPath = diffSections.ToDictionary(
            diff => diff.Key,
            diff => new MessageCreateParams {
                MaxTokens = maxTokens,
                Messages = [
                    new() {
                        Role = Role.User,
                        Content = $"Review this diff:\n```{diff.Value}```"
                    },
                ],
                Model = model,
                OutputConfig = new OutputConfig {
                    Format = new JsonOutputFormat { Schema = AnthropicSchema.FilterRequestSchema }, 
                },
                Temperature = 0.0,
            });

        Dictionary<string, ChatMessage> responses = await this.RequestReviewsAsync(requestsPerPath.Values.ToList());

        Dictionary<string, ChatMessage> reviewPerPath = responses.ToDictionary(

        )

        foreach(var (path, response) in requestsPerPath.Keys.Zip(responses)) {
            ((AnthropicTextDto)response).Inline.Path = path;
        }
        return responses;
    }  
 
}