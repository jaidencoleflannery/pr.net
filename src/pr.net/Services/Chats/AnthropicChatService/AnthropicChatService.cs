using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Messages;

using pr.net.Services.Chat.Instructions;

using pr.net.Configurations.Chat;

using pr.net.Models.Generic;
using pr.net.Models.Anthropic;
using pr.net.Models.Incoming.Anthropic;
using pr.net.Models.Incoming.Generic;

using static System.Text.Json.JsonSerializer;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class AnthropicChatService(
    IOptions<ChatConfiguration> _configuration, 
    IInstructionsService _instructionsService, 
    IAnthropicClient _client, 
    IAnthropicFilteringSchema _filterSchema, 
    IAnthropicReviewSchema _reviewSchema,
    ILogger<AnthropicChatService> _logger
) : IChatService { 

    private ChatProvider? _provider = _configuration.Value.Provider;

    public async Task<IEnumerable<DiffSection>?> FilterDiffsAsync(IEnumerable<DiffSection> diffSections) { 
        if(diffSections.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ Error processing pull request. No diff sections were given in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }
         
        if(_provider == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Error fetching AI provider in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        bool? useEmbedding = _configuration.Value.Filtering?.UseEmbedding;
        if(useEmbedding != null && useEmbedding == true)
            _logger.LogError($"\n{DateTime.Now}: [ Embedding has not been configured. ]\n"); // configure provider specific embedding service here.

        string? model = _configuration.Value.Filtering?.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:Model could not be found or read in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        long maxTokens = _configuration.Value.Filtering?.MaxTokens ?? 0;
        if(maxTokens <= 0 || maxTokens > 8192) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:MaxTokens could not be found or read, or is in an invalid format in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: true));
        if(instructions == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:MaxTokens could not be found or read, or is in an invalid format in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        // push schema into anthropic's required type for the format field.
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(Serialize(_filterSchema, _filterSchema.GetType())); 
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Schema could not be deserialized in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

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

    private async Task<IEnumerable<DiffSection>?> RequestFilteringAsync(IEnumerable<(DiffSection, MessageCreateParams)> requestsPerPath) {
        if(requestsPerPath.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ No diffs or paths provided to {nameof(RequestFilteringAsync)}. ]\n");
            return null;
        } 

        TimeSpan? timeout = _configuration.Value.Filtering?.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format in {nameof(RequestFilteringAsync)}. ]\n");
            return null;
        } 

        // iterate over every instance of requestDtos and send them individually.
        List<DiffSection> filteredDiffSections = [];
        List<Exception> exceptions = [];
        foreach((DiffSection section, MessageCreateParams request) in requestsPerPath) { 
            // note that the apikey is injected from the environment at init by the anthropic sdk.
            Message message;
            try {
                message = await _client.Messages.Create(request);
                if(message.Content[0].TryPickText(out TextBlock? textBlock)) {
                    // due to our output config, the response will be a single text block containing a json string with our boolean value.
                    var result = Deserialize<AnthropicisWorthReview>(textBlock?.Text!) ??
                        throw new InvalidOperationException("Could not parse filtering response.");
                    if(result.IsWorthReview == true)
                        filteredDiffSections.Add(section);
                }
            } catch(AnthropicApiException exception) {
                _logger.LogError($"\n{DateTime.Now}: [ Anthropic call failed: {exception.Message} in {nameof(RequestFilteringAsync)}. ]\n");
                exceptions.Add(exception);
            }
        }

        if(filteredDiffSections.Count > 0) {
            return filteredDiffSections;
        } else if(exceptions.Count > 0) {
            _logger.LogError($"\n{DateTime.Now}: [ No {nameof(RequestFilteringAsync)} calls were successfull, failed to perform review. ]\n");
            return null;
        } else {
            _logger.LogError($"\n{DateTime.Now}: [ No {nameof(RequestFilteringAsync)} calls were deemed worthy of review, short circuiting call. ]\n");
            return null;
        }
    } 

    public async Task<IEnumerable<(DiffSection, ChatResponse)>?> GetChatReviewsAsync(IEnumerable<DiffSection> diffSections) {
        if(diffSections.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ No diffs provided to {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        if(_provider != ChatProvider.Anthropic) {
            _logger.LogError($"\n{DateTime.Now}: [ Provider configuration does not match injected service in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }
 
        string? model = _configuration.Value.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Model could not be found or read in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        long maxTokens = _configuration.Value.MaxTokens ?? 0;
        if(maxTokens is <= 0 or > 8192) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:MaxTokens could not be found or read in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: false));
        if(string.IsNullOrWhiteSpace(instructions)) {
            _logger.LogError($"\n{DateTime.Now}: [ Could not fetch filtering instructions in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        // push schema into anthropic's required type for the format field.
        string schemaString = Serialize(_reviewSchema, _reviewSchema.GetType());
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(schemaString);
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Failure to serialize Anthropic Review schema in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

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
                        Model = model!,
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


    private async Task<List<(DiffSection, ChatResponse)>?> RequestReviewsAsync(List<(DiffSection, MessageCreateParams)> requestsPerPath) { 
        // iterate over every instance of requestDtos and send them individually.
        var reviewPerPath = new List<(DiffSection, ChatResponse)>();
        var exceptions = new List<Exception>();

        foreach((DiffSection section, MessageCreateParams parameter) in requestsPerPath) {
            if(parameter.Messages.Count < 1)
                continue;

            // note that the apikey is injected from the environment at init by the anthropic sdk.
            Message message;
            try {
                message = await _client.Messages.Create(parameter); 
                foreach(var content in message.Content) {
                    if(content.TryPickText(out TextBlock? textBlock) && textBlock != null) {
                        List<AnthropicReview>? reviews = JsonNode.Parse(textBlock!.Text)!["reviews"].Deserialize<List<AnthropicReview>>();
                        if(reviews == null)
                            throw new InvalidOperationException($"Could not parse text from response in {nameof(RequestReviewsAsync)}");
                        foreach(var review in reviews) {
                            AnthropicResponse response = new();
                            response.Content.Add(
                                new AnthropicContent() {
                                    Text = review.Body,
                                    Line = review.Line
                                });
                            reviewPerPath.Add((section, response));
                        }
                    }
                }
            } catch(AnthropicApiException exception) {
                _logger.LogError($"\n{DateTime.Now}: [ Anthropic call failed: {exception.Message} in {nameof(RequestReviewsAsync)}. ]\n");
                exceptions.Add(exception);
            }
        }

        return (reviewPerPath.Count > 0)
            ? reviewPerPath
            : null;
    }
 
}