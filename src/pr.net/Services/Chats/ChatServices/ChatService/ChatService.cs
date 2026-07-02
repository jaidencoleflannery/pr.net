using Microsoft.Extensions.Options;

using pr.net.Services.Chat.Instructions;
using pr.net.Services.Tooling;

using pr.net.Configurations.Chat;
using pr.net.Configurations.Tooling;

using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming;
using pr.net.Models.Schemas;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat;

public class ChatService(
    IOptions<ChatConfiguration> _chatConfiguration,
    IOptions<ToolingConfiguration> _toolingConfiguration,
    ILogger<ChatService> _logger,
    IToolingService _toolingService,
    IChatClient _chatClient,
    IInstructionsService _instructionsService
) : IChatService { 

    private uint _invocationCount = 0;

    private ChatProvider? _provider = _chatConfiguration.Value.Provider; 

    public async Task<IEnumerable<DiffSection>?> FilterDiffsAsync(IEnumerable<DiffSection> diffSections, string userId) { 
        if(diffSections.Count() < 1) {
            _logger.LogError($"\n{DateTime.Now}: [ Error processing pull request. No diff sections were given in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }
         
        if(_provider == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Error fetching AI provider in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        bool? useEmbedding = _chatConfiguration.Value.Filtering?.UseEmbedding;
        if(useEmbedding != null && useEmbedding == true)
            _logger.LogError($"\n{DateTime.Now}: [ Embedding has not been configured. ]\n"); // configure provider specific embedding service here.

        string? model = _chatConfiguration.Value.Filtering?.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:Model could not be found or read in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        long maxTokens = _chatConfiguration.Value.Filtering?.MaxTokens ?? 0;
        if(maxTokens <= 0 || maxTokens > 8192) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Filtering:MaxTokens could not be found or read, or is in an invalid format in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        } 

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: true));
        if(instructions == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Failed to fetch instructions in {nameof(FilterDiffsAsync)}. ]\n");
            return null;
        }

        TimeSpan? timeout = _chatConfiguration.Value.Filtering?.Timeout;
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
 
        string? model = _chatConfiguration.Value.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Model could not be found or read in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        long maxTokens = _chatConfiguration.Value.MaxTokens ?? 0;
        if(maxTokens is <= 0 or > 8192) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:MaxTokens could not be found or read in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        } 

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: false));
        if(string.IsNullOrWhiteSpace(instructions)) {
            _logger.LogError($"\n{DateTime.Now}: [ Could not fetch filtering instructions in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }  

        TimeSpan? timeout = _chatConfiguration.Value.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Configuration for Chat:Timeout could not be found or read, or is in an invalid format in {nameof(GetChatReviewsAsync)}. ]\n");
            return null;
        }

        return await _chatClient.RequestReviewsAsync(diffSections, maxTokens, model, instructions, timeout);
    }

    public async Task<List<DiffSection>?> GetChatContextAsync(IEnumerable<DiffSection> diffSections, PullReviewCreatedEvent prEvent, string userId) {
        if(diffSections.Count() < 1) {
            _logger.LogError($"{DateTime.Now}: No diffs provided to {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        string? model = _chatConfiguration.Value.Model;
        if(string.IsNullOrWhiteSpace(model)) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:Model could not be found or read in {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        long maxTokens = _chatConfiguration.Value.MaxTokens ?? 0;
        if(maxTokens is <= 0 or > 8192) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:MaxTokens was invalid or not found in {nameof(GetChatContextAsync)}.\n");
            return null;
        } 

        string? instructions = string.Join(' ', await _instructionsService.GetInstructions(isForFiltering: false));
        if(string.IsNullOrWhiteSpace(instructions)) {
            _logger.LogError($"\n{DateTime.Now}: Could not fetch instructions in {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        TimeSpan? timeout = _chatConfiguration.Value.Timeout;
        if(timeout == null) {
            _logger.LogError($"\n{DateTime.Now}: Configuration for Chat:Timeout could not be found or read, or is in an invalid format in {nameof(GetChatContextAsync)}.\n");
            return null;
        }

        uint fetchDepth = 0;
        uint totalSuccessfulInvocations = 0;
        List<(DiffSection, ToolingQuery)>? invocationRequestPerDiff = [];

        while(fetchDepth < _toolingConfiguration.Value.MaxInvocations) {
            // get tool invocation requests - accumulated invocation results are stored in diffSections.
            invocationRequestPerDiff = await this.GetRequestedToolInvocations(diffSections, maxTokens, model, instructions, timeout.Value); 
            if(invocationRequestPerDiff == null) {
                _logger.LogError($"\n{DateTime.Now}: Failed to get fetch tool invocations.");
                break;
            }

            uint numSuccessfulInvocations = await this.RunToolInvocations(invocationRequestPerDiff, prEvent); 
            if(numSuccessfulInvocations == 0) {
                _logger.LogError($"\n{DateTime.Now}: Failed to invoke tools.");
                break;
            }
            totalSuccessfulInvocations += numSuccessfulInvocations;

            ++fetchDepth;
        }

        return [..diffSections];
    }

    private async Task<List<(DiffSection, ToolingQuery)>?> GetRequestedToolInvocations(
        IEnumerable<DiffSection> diffSections,
        long maxTokens,
        string model,
        string instructions,
        TimeSpan timeout
    ) {
        List<(DiffSection, ToolingQuery)>? invocationRequestPerDiff = [];
        foreach(DiffSection diff in diffSections) {
            ToolingQuery? contextQueryResult = await _chatClient.QueryForToolUsage(diff, maxTokens, model, instructions, timeout);
            if(contextQueryResult == null) {
                _logger.LogError($"\n{DateTime.Now}: Failed to query for tool usage, response was null.");
                continue;
            }

            if(contextQueryResult.RunTool == true)
                invocationRequestPerDiff.Add((diff, contextQueryResult));
        }

        if(invocationRequestPerDiff.Count < 1) {
            _logger.LogInformation($"\n{DateTime.Now}: No tool invocations were requested.");
            return null;
        } else
            return invocationRequestPerDiff;
    }

    private async Task<uint> RunToolInvocations(
        IEnumerable<(DiffSection, ToolingQuery)> invocationRequestPerDiff,
        PullReviewCreatedEvent prEvent
    ) {
        if(invocationRequestPerDiff == null) {
            _logger.LogError($"\n{DateTime.Now}: {nameof(invocationRequestPerDiff)} was invalid.");
            return 0;
        }

        uint numSuccessfulInvocations = 0;

        // invoke. 
        foreach(var (diff, invocation) in invocationRequestPerDiff) {
            if(_invocationCount >= _toolingConfiguration.Value.MaxInvocations) {
                _logger.LogError($"\n{DateTime.Now}: Maximum number of tool invocations was reached, short circuiting path.");
                break;
            }

            // safety check, but these should already all be true.
            if(invocation.RunTool is null or false)
                continue;

            if(invocation.ToolId is null or < 0) {
                _logger.LogError($"\n{DateTime.Now}: Invalid ToolID was given: {invocation.ToolId}, skipping invocation.");
                continue;
            }

            _logger.LogInformation($"\n{DateTime.Now}: Tool invocation was requested for Tool ID: {invocation.ToolId}.");

            ToolParameters parameters = new(invocation.ToolId.Value, [invocation.ToolInput], prEvent, diff);
            ToolResponse toolResponse = await _toolingService.InvokeToolAsync(parameters);
            ++_invocationCount;
            if(!toolResponse.Success
            || toolResponse.Result.Count() < 1) {
                _logger.LogError($"\n{DateTime.Now}: Invocation failure for Tool ID: {invocation.ToolId}.");
                continue;
            }
            diff.Context.Add(toolResponse);
            ++numSuccessfulInvocations;
        }

        return numSuccessfulInvocations;
    }

}
