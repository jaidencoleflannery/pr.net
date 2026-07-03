using pr.net.Services.Tooling.Generic;
using pr.net.Services.Tooling;

using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Enums;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class EnvironmentReadFileTool(
    ILogger<EnvironmentReadFileTool> _logger, 
    IToolClient _toolClient
) : IReadFileTool {
    public ToolMetadata GetToolMetadata() => 
        new() {
            Name = nameof(ToolSignature.ReadFile),
            Description = $"Read a specified file from the repository, can only be used after {nameof(ToolSignature.ReadFileTree)} has been invoked.",
            IsChild = true,
            ToolPointer = this.InvokeTool
        };

    public async ValueTask<ToolResponse> InvokeTool(ToolParameters parameters) { 
        ToolResponse result = await _toolClient.FetchFile(parameters, GetToolMetadata()); 
        if(!result.Success
        || result.Result == null) {
            _logger.LogError($"{nameof(EnvironmentReadFileTool)}-{nameof(InvokeTool)}: Invocation of tool failed.");
            return ToolFail();
        }

        return result; 
    }

}
