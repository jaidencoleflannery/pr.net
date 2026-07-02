using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

using Anthropic;
using Anthropic.Models.Messages;
using Anthropic.Exceptions;

using pr.net.Services.Tooling;

using pr.net.Configurations.Tooling;

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
    IOptions<ToolingConfiguration> _toolingConfiguration,
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
                                Content = $"Review this diff:\n```{diff.Contents}```\n\nHere is the context that was gathered for this diff:\n{FormatContext(diff.Context)}"
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

    private static string FormatContext(IEnumerable<ToolResponse> context) {
        List<ToolResponse> successful = context.Where(c => c.Success && c.Result.Length > 0).ToList();
        if(successful.Count == 0)
            return "No additional context was gathered for this diff.";

        StringBuilder sb = new();
        sb.AppendLine("<context>");
        foreach(ToolResponse tool in successful) {
            sb.AppendLine($"  <tool name=\"{tool.ToolName}\">");
            foreach(string line in tool.Result)
                sb.AppendLine($"    {line}");
            sb.AppendLine("  </tool>");
        }
        sb.Append("</context>");
        return sb.ToString();
    }

    public async Task<ToolingQuery?> QueryForToolUsage(
        DiffSection diffSection,
        long maxTokens,
        string model,
        string instructions,
        TimeSpan? timeout
    ) {
        if(diffSection == null
        || string.IsNullOrWhiteSpace(model) 
        || maxTokens < 1) {
            _logger.LogError($"\n{DateTime.Now}: Provided parameters were invalid in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        if(string.IsNullOrWhiteSpace(diffSection.Contents)) {
            _logger.LogInformation($"\n{DateTime.Now}: File ({diffSection.Path})'s contents were empty in {nameof(QueryForToolUsage)}.\n");
            return null;
        }
 
        if(diffSection.Contents.Length > 100000) {
            _logger.LogInformation($"\n{DateTime.Now}: File ({diffSection.Path}) was too large to run tools on in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        // push schema into anthropic's required type for the format field.
        string schemaString = Serialize(_toolingSchema, _toolingSchema.GetType());
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(schemaString);
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: Failure to serialize Tooling Query schema in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        // fetch available tools.
        Dictionary<ToolSignature, ToolMetadata> availableTools = _toolingService.GetOptionalTools(); 

        string prompt = 
            // diff files.
            $"You will be reviewing a diff, but first, you need to gather all the necesarry context.\n"
            + $"Your goal is to gather the most important context. Keep in mind that you can only make a maximum of {_toolingConfiguration.Value.MaxInvocations} tool invocations."
            + $"Here is the diff:\n"
            + $"```\nPath: {diffSection.Path}.\nContents: {diffSection.Contents}\n```\n"
            // tools.
            + $"Tools:\n"
            + $"For tools, note that some tools can only be called after their parent is called. Each time you are prompted, you can only invoke one tool, then the result of the tool will be given back to you so you can invoke another.\n"
            + $"Here are your available tools, their associated descriptions, and their ID for invocation:\n"
            + $"```\n{string.Join("; \n", availableTools.Select(keyToolPair => $"Tool: {{ \nID: {(int)keyToolPair.Key}. \nName: {keyToolPair.Value.Name}. \nDescription: {keyToolPair.Value.Description}. \n}}"))}``` \n"
            + $"If you'd like to run a tool:\n"
            + $"Set the \"RunTool\" field to true.\n"
            + $"Set the \"ToolId\" field with the ID of the tool you'd like to run.\n"
            + $"Set the \"ToolInput\" field to the required input (leave blank if no input is needed).\n"
            // previous invocations.
            + $"If you have previously ran any tools, their results are appended:\n"
            + $"```\n{diffSection.Context.Select(invocation => $"Tool: {{ \nName: {invocation.ToolName}. \nDescription: {invocation.Description}. \nResult: {string.Join(", ", invocation.Result)}. \n}}")}```\n"
            + $"If a tool has already ran, do not try it again unless you have evidence it will succeed on retry.";

        MessageCreateParams requestParameter = new() {
            MaxTokens = maxTokens,
            // we avoid the tool field in the api so we have full control over flow. 
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
        }; 

        // note that the apikey is injected from the environment at init by the anthropic sdk.
        ToolingQuery toolingQueryResult = new();
        Message message;
        try {
            message = await _client.Messages.Create(requestParameter); 
            foreach(ContentBlock content in message.Content) {
                if(content.TryPickText(out TextBlock? textBlock) && textBlock != null) {
                    ToolingQuery? response = JsonNode.Parse(textBlock!.Text)!.Deserialize<ToolingQuery>()
                        ?? throw new InvalidOperationException($"Could not parse text from response in {nameof(QueryForToolUsage)}");
                    toolingQueryResult = response;
                } else {
                    _logger.LogError($"\n{DateTime.Now}: Anthropic call could not be made, could not parse text block from request.");
                    continue;
                }
            }
        } catch(AnthropicApiException exception) {
            _logger.LogError($"\n{DateTime.Now}: Anthropic call failed: {exception.Message} in {nameof(QueryForToolUsage)}.\n");
            return null;
        }

        return toolingQueryResult;
    }

}

