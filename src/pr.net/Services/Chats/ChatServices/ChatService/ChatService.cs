using Microsoft.Extensions.Options;

using pr.net.Services.Chat.Instructions;
using pr.net.Services.Tooling;

using pr.net.Configurations.Chat;

using pr.net.Models.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Schemas;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class ChatService(
    IOptions<ChatConfiguration> _configuration, 
    ILogger<ChatService> _logger,
    IToolingService _toolingService,
    IChatClient _chatClient,
    IInstructionsService _instructionsService
) : IChatService { 

    private ChatProvider? _provider = _configuration.Value.Provider;

    public async Task<IEnumerable<DiffSection>?> FilterDiffsAsync(IEnumerable<DiffSection> diffSections, string userId) { 
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
            _logger.LogError($"\n{DateTime.Now}: [ Failed to fetch instructions in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        TimeSpan? timeout = _configuration.Value.Filtering?.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        } 

        return await _chatClient.RequestFilteringAsync(diffSections, maxTokens, model, instructions, timeout); 
    }

    public async Task<IEnumerable<(DiffSection, ChatResponse)>?> GetChatReviewsAsync(IEnumerable<DiffSection> diffSections, string userId) {
        if(diffSections.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ No diffs provided to {nameof(GetChatReviewsAsync)}. ]\n");
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

        TimeSpan? timeout = _configuration.Value.Filtering?.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        return await _chatClient.RequestReviewsAsync(diffSections, maxTokens, model, instructions, timeout);
    }

    public async Task<IEnumerable<DiffSection>?> GetChatContextAsync(
        IEnumerable<DiffSection> diffSections, 
        string userId
    ) {
        if(diffSections.Count() < 1) {
            _logger.LogError($"{DateTime.Now}: No diffs provided to {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        string? model = _configuration.Value.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:Model could not be found or read in {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        long maxTokens = _configuration.Value.MaxTokens ?? 0;
        if(maxTokens is <= 0 or > 8192) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:MaxTokens could not be found or read in {nameof(GetChatContextAsync)}.\n");
            return null;
        } 

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: false));
        if(string.IsNullOrWhiteSpace(instructions)) {
            _logger.LogError($"\n{DateTime.Now}: Could not fetch filtering instructions in {nameof(GetChatContextAsync)}.\n");
            return null;
        }  

        TimeSpan? timeout = _configuration.Value.Filtering?.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:Filtering:Timeout could not be found or read, or is in an invalid format in {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        List<ToolingQuery> invocationRequests = [];
        foreach(DiffSection diff in diffSections) {
            ToolingQuery? contextQueryResult = await _chatClient.QueryForToolUsage(diff, maxTokens, model, instructions, timeout);
            if(contextQueryResult == null) {
                _logger.LogError($"\n{DateTime.Now}: Failed to query for tool usage, response was null.");
                continue;
            }

            if(contextQueryResult.RunTool != null && contextQueryResult.RunTool == true)
                invocationRequests.Add(contextQueryResult);
        }

        if(invocationRequests.Count < 1) {
            _logger.LogInformation($"\n{DateTime.Now}: No tool invocations were requested.");
            return null;
        }

        foreach(ToolingQuery invocation in invocationRequests) {
            // safety check, but these should already all be true.
            if(invocation.RunTool == null 
            || invocation.RunTool == false)
                continue;

            if(invocation.RunTool == true
            && invocation.ToolId != null) {
                _logger.LogInformation($"\n{DateTime.Now}: Tool invocation was requested for Tool ID: {invocation.ToolId}.");
                ToolResponse toolResponse = await _toolingService.InvokeToolAsync(invocation.ToolId);
            }
        }

        return invocationRequests;
    } 
}

