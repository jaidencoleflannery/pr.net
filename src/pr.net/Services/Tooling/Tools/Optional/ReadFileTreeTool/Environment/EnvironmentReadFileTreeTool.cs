using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Enums;
using pr.net.Models.Generic;
using pr.net.Models.Outbound.Generic;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class EnvironmentReadFileTreeTool(
        IToolClient _toolClient,
        ILogger<EnvironmentReadFileTreeTool> _logger
    ) : IReadFileTreeTool {

    public ToolMetadata GetToolMetadata() => 
        new ToolMetadata {
            Name = ToolSignature.ReadFileTree.ToString(), 
            Description = "Get repository directory tree.",
            ToolPointer = this.InvokeTool
        };

    public async ValueTask<ToolResponse> InvokeTool(PullReviewCreatedMetadata metadata, DiffSection[]? diffSections = null) {
        ToolResponse toolResponse = await ReadFileTree(metadata);
        if(!toolResponse.Success
        || toolResponse.Result == null) {
            _logger.LogError($"{nameof(ReadFileTree): Invocation of tool failed.}");
            return ToolFail();
        }

        return toolResponse;
    }

    private async Task<ToolResponse> ReadFileTree(PullReviewCreatedMetadata prEvent) =>
        await _toolClient.FetchFileTree(prEvent);

}

