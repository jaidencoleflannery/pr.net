using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class EnvironmentReadFileTreeTool(
        IToolClient _toolClient,
        ILogger<EnvironmentReadFileTreeTool> _logger
    ) : IReadFileTreeTool {

    public async ValueTask<ToolResponse> InvokeTool(ToolParameters parameters) {
        if(parameters is not ReadFileTreeParameters input
        || input.prEvent == null) {
            _logger.LogError($"{nameof(ReadFileTree)}: Failed to invoke tool, parameters given were invalid");
            return ToolFail();
        }

        ToolResponse toolResponse = await ReadFileTree(input.prEvent);
        if(!toolResponse.Success
        || toolResponse.Result == null) {
            _logger.LogError($"{nameof(ReadFileTree): Invocation of tool failed.}");
            return ToolFail();
        }

        return toolResponse;
    }

    public async Task<ToolResponse> ReadFileTree(PullReviewCreatedEvent prEvent) =>
        await _toolClient.FetchFileTree(prEvent);

}

