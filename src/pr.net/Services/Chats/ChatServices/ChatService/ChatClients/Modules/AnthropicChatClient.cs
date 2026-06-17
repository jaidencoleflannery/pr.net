using System.Text.Json;
using System.Text.Json.Nodes;

using Anthropic;
using Anthropic.Models.Messages;
using Anthropic.Exceptions;

using pr.net.Services.Tooling;

using pr.net.Models.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Incoming.Anthropic;
using pr.net.Models.Schemas;
using pr.net.Models.Tooling;
using pr.net.Models.Enums;

using static System.Text.Json.JsonSerializer;

namespace pr.net.Services.Chat;

public class AnthropicChatClient(
    ILogger<AnthropicChatClient> _logger,
    IToolingService _toolingService,
    IAnthropicClient _client,
    IFilteringSchema _filterSchema, 
    IReviewSchema _reviewSchema,
    IToolingSchema _toolingSchema 
) : IChatClient {
    
    public async Task<IEnumerable<DiffSection>?> RequestFilteringAsync(
        IEnumerable<DiffSection> diffSections, 
        long maxTokens, 
        string model, 
        string instructions,
        TimeSpan? timeout
    ) {
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: Parameter for model was invalid in {nameof(RequestFilteringAsync)}.\n");
            return null;
        }

        // push schema into provider's required type for the format field.
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(Serialize(_filterSchema, _filterSchema.GetType())); 
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Schema could not be deserialized in {nameof(RequestFilteringAsync)}. ]\n");
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
                                Content = $"Is this diff worth reviewing?:\n```{diff.Contents}```"
                            },
                        ],
                        Model = model,
                        OutputConfig = new OutputConfig {
                            Format = new JsonOutputFormat { 
                                Schema = schema 
                            }, 
                        },
                        System = instructions,
                        Temperature = 0.0,
                    }));
        }

        if(requestsPerPath.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ No diffs or paths provided to {nameof(RequestFilteringAsync)}. ]\n");
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
                    var result = Deserialize<FilteringResponse>(textBlock?.Text!) ??
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

    public async Task<List<(DiffSection, ChatResponse)>?> RequestReviewsAsync(
        IEnumerable<DiffSection> diffSections,
        long maxTokens, 
        string model, 
        string instructions,
        TimeSpan? timeout
    ) {
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: Parameter for model was invalid in {nameof(RequestReviewsAsync)}.\n");
            return null;
        }

        // push schema into anthropic's required type for the format field.
        string schemaString = Serialize(_reviewSchema, _reviewSchema.GetType());
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(schemaString);
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Failure to serialize Anthropic Review schema in {nameof(RequestReviewsAsync)}. ]\n");
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
                        System = instructions,
                        Temperature = 0.0,
                    }));
        }

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
                foreach(ContentBlock content in message.Content) {
                    if(content.TryPickText(out TextBlock? textBlock) && textBlock != null) {
                        List<Review>? response = JsonNode.Parse(textBlock!.Text)!["reviews"].Deserialize<List<Review>>()
                            ?? throw new InvalidOperationException($"Could not parse text from response in {nameof(RequestReviewsAsync)}");
                        foreach(Review review in response) {
                            AnthropicResponse result = new();
                            result.Content.Add(
                                new ChatContent() {
                                    Text = review.Body,
                                    Line = review.Line
                                });
                            reviewPerPath.Add((section, result));
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

    public async Task<List<(DiffSection, ToolResponse)>?> QueryForToolUsage(
        IEnumerable<DiffSection> diffSections,
        long maxTokens,
        string model,
        string instructions,
        TimeSpan? timeout
    ) {
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: Parameter for model was invalid in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        // push schema into anthropic's required type for the format field.
        // this is our desired structured output.
        string schemaString = Serialize(_toolingSchema, _toolingSchema.GetType());
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(schemaString);
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: Failure to serialize Tooling schema in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        // fetch available tools.
        Dictionary<ToolSignature, ToolMetadata> availableTools = _toolingService.GetOptionalTools();
 
        // build all requests.
        List<(DiffSection, MessageCreateParams)> requestsPerPath = [];
        foreach(DiffSection diff in diffSections) {
            if(diff.Contents.Length > 100000) {
                _logger.LogInformation($"\n{DateTime.Now}: File ({diff.Path}) was too large to run tools on in {nameof(QueryForToolUsage)}.\n");
                continue;
            }

            if(!string.IsNullOrWhiteSpace(diff.Contents)) {
                string prompt = 
                    // diff files.
                    $"You will be reviewing a diff, but first, you need to gather all the necesarry context.\n" +
                    $"Here is the diff:\n" +
                    $"```\nPath: {diff.Path}.\nContents: {diff.Contents}\n```\n" +
                    // tools.
                    $"For tools, note that some tools can only be called after their parent is called.\n" +
                    $"Here are your available tools and their associated descriptions:\n" +
                    $"```\n{availableTools.Values.Select(tool => $"Tool: {{ {tool.Name}.\n Description: {tool.Description}.\n }}\n")}```\n";
                
                requestsPerPath.Add(
                    (diff,
                    new MessageCreateParams {
                        MaxTokens = maxTokens,
                        Messages = [
                            new() {
                                Role = Role.User,
                                Content = prompt
                            },
                        ],
                        Model = model!,
                        OutputConfig = new OutputConfig {
                            Format = new JsonOutputFormat { 
                                Schema = schema 
                            }, 
                        },
                        System = instructions,
                        Temperature = 0.0,
                    }
                ));
            }
        }

        // iterate over every request and send them individually.
        List<(DiffSection, ChatResponse)> reviewPerPath = [];
        var exceptions = new List<Exception>();

        foreach((DiffSection section, MessageCreateParams parameter) in requestsPerPath) { 
            // note that the apikey is injected from the environment at init by the anthropic sdk.
            Message message;
            try {
                message = await _client.Messages.Create(parameter); 
                foreach(ContentBlock content in message.Content) {
                    if(content.TryPickText(out TextBlock? textBlock) && textBlock != null) {
                        List<Review>? response = JsonNode.Parse(textBlock!.Text)!["reviews"].Deserialize<List<Review>>()
                            ?? throw new InvalidOperationException($"Could not parse text from response in {nameof(RequestReviewsAsync)}");
                        foreach(Review review in response) {
                            AnthropicResponse result = new(); // TODO: this most likely isn't going to work. needs to be the model.
                            result.Content.Add(
                                new ChatContent() {
                                    Text = review.Body,
                                    Line = review.Line
                                });
                            reviewPerPath.Add((section, result));
                        }
                    } else {
                        _logger.LogError($"\n{DateTime.Now}: Anthropic call could not be made, could not parse text block from request.");
                        continue;
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

