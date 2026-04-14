using System.Text.Json;
using System.Text.Json.Nodes;

using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Schemas;

using static System.Text.Json.JsonSerializer;

namespace pr.net.Services.Chat;

public class AmazonChatClient(
    ILogger<AmazonChatClient> _logger,
    IAmazonBedrockRuntime _client,
    IFilteringSchema _filterSchema,
    IReviewSchema _reviewSchema
) : IChatClient {
    
    public async Task<IEnumerable<DiffSection>?> RequestFilteringAsync(
        IEnumerable<DiffSection> diffSections, 
        long maxTokens, 
        string model, 
        string instructions,
        TimeSpan? timeout
    ) {
        // push schema into provider's required type for the format field.
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(Serialize(_filterSchema, _filterSchema.GetType())); 
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Schema could not be deserialized in {nameof(RequestFilteringAsync)}. ]\n");
            return null;
        }

        List<(DiffSection, ConverseRequest)> requestsPerPath = []; 
        foreach(DiffSection diff in diffSections) {
            if(!string.IsNullOrWhiteSpace(diff.Contents))
                requestsPerPath.Add((diff, new ConverseRequest {
                    ModelId = model,
                    Messages = new List<Message> {
                        new Message {
                            Role = ConversationRole.User,
                            Content = new List<ContentBlock> {
                                new ContentBlock { Text = $"Is this diff worth reviewing?:\n```{diff.Contents}```" }
                            }
                        }
                    },
                    OutputConfig = new OutputConfig {
                        TextFormat = new OutputFormat {
                            Type = "json_schema",
                            Structure = new OutputFormatStructure {
                                JsonSchema = new JsonSchemaDefinition {
                                    Schema = JsonSerializer.Serialize(_filterSchema)
                                }
                            }
                        }
                    },
                    InferenceConfig = new InferenceConfiguration {
                        MaxTokens = (int)maxTokens,
                        Temperature = 0.0F
                    },
                    System = [new SystemContentBlock {
                        Text = instructions
                    }]
            }));
        };

        if(requestsPerPath.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ No diffs or paths provided to {nameof(RequestFilteringAsync)}. ]\n");
            return null;
        }  

        // iterate over every instance of requests and send them individually.
        List<DiffSection> filteredDiffSections = [];
        List<Exception> exceptions = []; 

        foreach((DiffSection section, ConverseRequest request) in requestsPerPath) { 
            ConverseResponse response;
            try {
                response = await _client.ConverseAsync(request);
                string message = response.Output?.Message?.Content?[0]?.Text ?? ""; 
                FilteringResponse result = JsonSerializer.Deserialize<FilteringResponse>(message)
                    ?? throw new InvalidOperationException($"[ Could not deserialize response in ${RequestFilteringAsync}]");
                if(result.IsWorthReview == true)
                    filteredDiffSections.Add(section);
            } catch(Exception exception) {
                _logger.LogError($"[ Amazon call failed: {exception.Message} in {nameof(RequestFilteringAsync)}. ]\n");
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
        long maxTokens, string model, 
        string instructions,
        TimeSpan? timeout
    ) {
        // push schema into anthropic's required type for the format field.
        string schemaString = Serialize(_reviewSchema, _reviewSchema.GetType());
        Dictionary<string, JsonElement>? schema = Deserialize<Dictionary<string, JsonElement>>(schemaString);
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Failure to serialize Anthropic Review schema in {nameof(RequestReviewsAsync)}. ]\n");
            return null;
        }

        List<(DiffSection, ConverseRequest)> requestsPerPath = []; 
        foreach(DiffSection diff in diffSections) {
            if(!string.IsNullOrWhiteSpace(diff.Contents))
                requestsPerPath.Add((diff, new ConverseRequest {
                    ModelId = model,
                    Messages = new List<Message> {
                        new Message {
                            Role = ConversationRole.User,
                            Content = new List<ContentBlock> {
                                new ContentBlock { Text = $"Review this diff:\n```{diff.Contents}```" }
                            }
                        }
                    },
                    OutputConfig = new OutputConfig {
                        TextFormat = new OutputFormat {
                            Type = "json_schema",
                            Structure = new OutputFormatStructure {
                                JsonSchema = new JsonSchemaDefinition {
                                    Schema = JsonSerializer.Serialize(_reviewSchema)
                                }
                            }
                        }
                    },
                    InferenceConfig = new InferenceConfiguration {
                        MaxTokens = (int)maxTokens,
                        Temperature = 0.0F
                    },
                    System = [new SystemContentBlock {
                        Text = instructions
                    }]
            }));
        };

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



        foreach((DiffSection section, ConverseRequest request) in requestsPerPath) { 
            ConverseResponse response;
            try {
                response = await _client.ConverseAsync(request);
                string message = response.Output?.Message?.Content?[0]?.Text ?? ""; 
                ReviewResponse result = JsonSerializer.Deserialize<ReviewResponse>(message)
                    ?? throw new InvalidOperationException($"[ Could not deserialize response in ${RequestFilteringAsync}]");
                if(result.IsWorthReview == true)
                    filteredDiffSections.Add(section);
            } catch(Exception exception) {
                _logger.LogError($"[ Amazon call failed: {exception.Message} in {nameof(RequestFilteringAsync)}. ]\n");
                exceptions.Add(exception);
            }
        }



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