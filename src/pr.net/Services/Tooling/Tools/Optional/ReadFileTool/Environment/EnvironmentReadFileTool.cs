using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Enums;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class EnvironmentReadFileTool( 
        ILogger<EnvironmentReadFileTool> _logger,
        IToolClient _toolClient
    ) : IReadFileTool {

    public ToolMetadata GetToolMetadata() => 
        new ToolMetadata {
            Name = ToolSignature.ReadFile.ToString(),
            Description = $"Read a specified file from the repository, can only be used after {ToolSignature.ReadFileTree.ToString()} has been invoked.",
            IsChild = true,
            ToolPointer = this.InvokeTool 
        };

    public async ValueTask<ToolResponse> InvokeTool(ToolParameters parameters) {
        if(parameters is not ReadFileParameters input
        || input.PrEvent == null
        || input.FilePath == null) {
            _logger.LogError($"{nameof(ReadFile)}: Failed to invoke tool, parameters given were invalid");
            return ToolFail();
        }

        ToolResponse result = await ReadFile(input.PrEvent, input.FilePath);
        if(!result.Success
        || result.Result == null) {
            _logger.LogError($"{nameof(ReadFile): Invocation of tool failed.}");
            return ToolFail();
        }

        return new ToolResponse {
            Success = result.Success,
            Result = result.Result
        };
    }

    public async Task<ToolResponse> ReadFile(PullReviewCreatedEvent prEvent, string filePath) =>
        await _toolClient.FetchFile(prEvent, filePath); 

}

