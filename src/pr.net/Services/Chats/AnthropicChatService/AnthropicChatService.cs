using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.AI;

using Anthropic;

using Anthropic.Exceptions;
using Anthropic.Models.Messages;

using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;

using pr.net.Models.Generic;
using pr.net.Models.Anthropic;
using pr.net.Models.Incoming.Anthropic;

using static System.Text.Json.JsonSerializer;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(IConfiguration _configuration, IInstructionsService _instructionsService, IAnthropicClient _client, ITokenService _tokenService) : IChatService { 

    public async Task<IEnumerable<DiffSection>> FilterDiffsAsync(IList<DiffSection> diffSections) { 
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

        // push schema into anthropic's required type for the format field.
        Dictionary<string, JsonElement> schema = Deserialize<Dictionary<string, JsonElement>>(Serialize(new AnthropicSchema<AnthropicFilteringProperties>()))
            ?? throw new InvalidOperationException("Failure to serialize Anthropic Filtering schema.");

        // requestsperpath's key == (path, contents), value == request. 
        List<(DiffSection, MessageCreateParams)> requestsPerPath = [];
        foreach(DiffSection diff in diffSections) {
            if(!string.IsNullOrWhiteSpace(diff.Contents))
                requestsPerPath.Add(
                    (diff, 
                    new MessageCreateParams {
                        MaxTokens = maxTokens,
                        Messages = [
                            new() {
                                Role = Role.User,
                                Content = $"Is this diff worth reviewing?:\n```{diff.Contents}```"
                            },
                        ],
                        Model = model,
                        OutputConfig = new OutputConfig {
                            Format = new JsonOutputFormat { 
                                Schema = schema 
                            }, 
                        },
                        Temperature = 0.0,
                    }));
        }

        return await this.RequestFilteringAsync(requestsPerPath);
    }

    private async Task<List<DiffSection>> RequestFilteringAsync(List<(DiffSection, MessageCreateParams)> requestsPerPath) {
        if(requestsPerPath.Count < 1)
            throw new InvalidOperationException($"No diffs or paths provided to {nameof(RequestFilteringAsync)}");

        if(!TimeSpan.TryParse(_configuration["Chat:Filtering:Timeout"], out TimeSpan timeout))
            throw new InvalidOperationException("Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format."); 

        // iterate over every instance of requestDtos and send them individually.
        List<DiffSection> filteredDiffSections = [];
        List<Exception> exceptions = [];
        foreach((DiffSection section, MessageCreateParams request) in requestsPerPath) { 
            Message message;
            try {
                message = await _client.Messages.Create(request);
                if(message.Content[0].TryPickText(out TextBlock? textBlock)) {
                    // due to our output config, the response will be a single text block containing a json string with our boolean value.
                    Dictionary<string, JsonElement> result = Deserialize<Dictionary<string, JsonElement>>(textBlock!.Text)!;
                    if(result?["isWorthReview"].GetBoolean() == true)
                        filteredDiffSections.Add(section);
                }
            } catch(AnthropicApiException exception) {
                Console.WriteLine($"Anthropic call failed: {exception.Message}");
                exceptions.Add(exception);
            }    
        }

        if(filteredDiffSections.Count > 0)
            return filteredDiffSections;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    } 

    public async Task<List<(DiffSection, AnthropicResponse)>> GetChatReviewsAsync(List<DiffSection> diffSections) {
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

        // push schema into anthropic's required type for the format field.
        Dictionary<string, JsonElement> schema = Deserialize<Dictionary<string, JsonElement>>(Serialize(new AnthropicSchema<AnthropicReviewProperties>()))
            ?? throw new InvalidOperationException("Failure to serialize Anthropic Review schema.");

        List<(DiffSection, MessageCreateParams)> requestsPerPath = [];
        foreach(DiffSection diff in diffSections) {
            if(!string.IsNullOrWhiteSpace(diff.Contents))
                requestsPerPath.Add(
                    (diff, 
                    new MessageCreateParams {
                        MaxTokens = maxTokens,
                        Messages = [
                            new() {
                                Role = Role.User,
                                Content = $"Review this diff:\n```{diff.Contents}```"
                            },
                        ],
                        Model = model,
                        OutputConfig = new OutputConfig {
                            Format = new JsonOutputFormat { 
                                Schema = schema 
                            }, 
                        },
                        Temperature = 0.0,
                    }));
        }

        return await this.RequestReviewsAsync(requestsPerPath);
    }


    private async Task<List<(DiffSection, AnthropicResponse)>> RequestReviewsAsync(List<(DiffSection, MessageCreateParams)> requestsPerPath) { 
        // iterate over every instance of requestDtos and send them individually.
        var reviewPerPath = new List<(DiffSection, AnthropicResponse)>();
        var exceptions = new List<Exception>();

        foreach((DiffSection section, MessageCreateParams parameter) in requestsPerPath) {
            if(parameter.Messages.Count < 1)
                continue;

            Message message;
            try {
                message = await _client.Messages.Create(parameter); 
                if(message.Content[0].TryPickText(out TextBlock? textBlock)) {
                    AnthropicResponse response = new();
                    response.Content.Add(
                        new AnthropicContent() {
                            Text = JsonNode.Parse(textBlock!.Text)!["review"]!.GetValue<string>()
                        });
                    reviewPerPath.Add((section, response));
                }
            } catch(AnthropicApiException exception) {
                Console.WriteLine($"Anthropic call failed: {exception.Message}");
                exceptions.Add(exception);
            }    
        }

        return (reviewPerPath.Count > 0)
            ? reviewPerPath
            : throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }
 
}