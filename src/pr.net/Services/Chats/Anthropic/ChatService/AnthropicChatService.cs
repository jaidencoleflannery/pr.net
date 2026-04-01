using System.Text;

using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;

using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Chat.Generic;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Anthropic;
using pr.net.Models.Incoming.Anthropic;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(IConfiguration _configuration, IInstructionsService _instructionsService, IAnthropicClient _client, ITokenService _tokenService) : IChatService { 

    public async Task<Dictionary<string, string>> FilterDiffsAsync(Dictionary<string, string> diffSections) { 
        if(diffSections.Count < 1)
            throw new InvalidOperationException($"No diffs provided to {nameof(FilterDiffsAsync)}.");
        
        ChatProvider? provider = ValidateChatProvider(_configuration["Chat:Provider"]);
        if(provider != ChatProvider.Anthropic)
            throw new InvalidOperationException("Provider configuration does not match injected service.");

        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}.");

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
        var requestsPerPath = diffSections.ToDictionary(
            diff => diff.Key,
            diff => new MessageCreateParams {
                MaxTokens = maxTokens,
                Messages = [
                    new() {
                        Role = Role.User,
                        Content = $"Is this diff worth reviewing?\n```{diff.Value}```"
                    },
                ],
                Model = model,
                OutputConfig = new OutputConfig {
                    Format = new JsonOutputFormat { Schema = AnthropicSchema.FilterRequestSchema }
                }
            }); 

        List<ChatResponseText> filterResponses = await this.RequestFilteringAsync([..requestsPerPath.Values], url);
        var keysToRemove = diffSections
            .Keys.Zip(filterResponses)
            .Where(pair => pair.Second is AnthropicFilteringTextDto dto && !dto.Content.Raw)
            .Select(pair => pair.First)
            .ToList();

        foreach(var key in keysToRemove)
            diffSections.Remove(key);

        return diffSections;
    }

    public async Task<List<ChatResponseText>> RequestFilteringAsync(List<MessageCreateParams> parameters, string url) {
        // iterate over every instance of requestDtos and send them individually.
        var responses = new List<ChatResponseText>();
        var exceptions = new List<Exception>();
        System.Uri targetUrl = new Uri(url);
        foreach(MessageCreateParams parameter in parameters) {
            if(parameter.Messages.Count < 1)
                continue;

            Message message;
            try {
                message = await _client.Messages.Create(parameter);
                responses.Add(message.Content[0].Value); 
            } catch(AnthropicApiException exception) {
                Console.WriteLine($"Anthropic call failed: {exception.Message}");
                exceptions.Add(exception);
            }  

                    else
                        Console.WriteLine("Failed to parse chat response for filtering.");
                } else {
                    Exception exception = new Exception($"Request for filtering failed: {responseDto}");
                    Console.WriteLine(exception);
                    exceptions.Add(exception);
                }
            }
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }

    public async Task<List<ChatResponseText>> RequestReviewsAsync(List<AnthropicRequestDto> requestDtos, string url) { 
        // iterate over every instance of requestDtos and send them individually  
        var responses = new List<ChatResponseText>();
        var exceptions = new List<Exception>();

        foreach(var requestDto in requestDtos) {
            if(requestDto.Messages.Count < 1)
                continue;

            using (var message = new HttpRequestMessage(HttpMethod.Post, targetUrl)) { 
                message.Headers.Add("x-api-key", token);
                message.Headers.Add("anthropic-version", "2023-06-01");
                var json = JsonSerializer.Serialize(requestDto);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(message); 
                string responseString = await response.Content.ReadAsStringAsync();
                AnthropicResponseDto responseDto = JsonSerializer.Deserialize<AnthropicResponseDto>(responseString)!;
                AnthropicTextDto textDto = JsonSerializer.Deserialize<AnthropicTextDto>(responseDto.Content[0].Text)!;

                if(response.IsSuccessStatusCode) {
                    if(textDto != null)
                        responses.Add(textDto);
                    else
                        Console.WriteLine("Failed to parse chat response.");
                } else {
                    Exception exception = new Exception($"Request for review failed: {responseDto}");
                    Console.WriteLine(exception);
                    exceptions.Add(exception);
                }
            }
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
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