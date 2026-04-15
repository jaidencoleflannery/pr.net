using System.Text.Json;

using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;

using pr.net.Models.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Schemas;
using pr.net.Models.Incoming.Amazon;

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
        string schema = Serialize(_filterSchema, _filterSchema.GetType());
        if(schema == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Schema could not be deserialized in {nameof(RequestFilteringAsync)}. ]\n");
            return null;
        }

        List<DiffSection> diffs = [..diffSections];

        Console.WriteLine($"\n{schema}\n");
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
                                    Name = "filter_result",
                                    Schema = schema
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
                FilteringResponse result = Deserialize<FilteringResponse>(message)
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
        string schema = Serialize(_reviewSchema, _reviewSchema.GetType());
        if(string.IsNullOrWhiteSpace(schema)) {
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
                                    Schema = schema
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

        // iterate over every instance of requestDtos and send them individually.
        var reviewPerPath = new List<(DiffSection, ChatResponse)>();
        var exceptions = new List<Exception>();

        foreach((DiffSection section, ConverseRequest request) in requestsPerPath) { 
            ConverseResponse response;
            try {
                response = await _client.ConverseAsync(request); 
                ReviewResponse message = Deserialize<ReviewResponse>(response.Output?.Message?.Content?[0]?.Text ?? "")
                    ?? throw new InvalidOperationException($"[ Could not deserialize response in ${RequestFilteringAsync}]");  
                foreach(Review review in message.Reviews!) {
                    AmazonResponse currentReview = new();
                    currentReview.Content.Add(
                        new ChatContent() {
                            Text = review.Body,
                            Line = review.Line
                        });
                    reviewPerPath.Add((section, currentReview));
                }
            } catch(Exception exception) {
                _logger.LogError($"[ Amazon call failed: {exception.Message} in {nameof(RequestFilteringAsync)}. ]\n");
                exceptions.Add(exception);
            }
        } 

        return (reviewPerPath.Count > 0)
            ? reviewPerPath
            : null;
    }
        
}