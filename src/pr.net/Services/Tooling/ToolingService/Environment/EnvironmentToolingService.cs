using pr.net.Models.Enums;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ToolSignature;

namespace pr.net.Services.Tooling;

public class EnvironmentToolingService : IToolingService {

    // all added tools need to be populated within the ToolSignature enum and have a populated ToolMetadata instance to be added to the mapping.
    private static Dictionary<ToolSignature, ToolMetadata> _requiredToolMap = [];
    private static Dictionary<ToolSignature, ToolMetadata> _optionalToolMap = [];

    private readonly IReadFileTreeTool _readFileTreeTool;
    private readonly IReadFileTool _readFileTool;

    private readonly ILogger<EnvironmentToolingService> _logger;

    private readonly ToolResponse toolFailure = new(success: false, toolName: string.Empty, result: []);

    public EnvironmentToolingService(
        IReadFileTreeTool readFileTreeTool, 
        IReadFileTool readFileTool,
        ILogger<EnvironmentToolingService> logger
    ) {
        _readFileTreeTool = readFileTreeTool;
        _readFileTool = readFileTool;

        _requiredToolMap = new() { };
        _optionalToolMap = new() {
            [ReadFileTree] = _readFileTreeTool.GetToolMetadata(),
            [ReadFile] = _readFileTool.GetToolMetadata() 
        };

        _logger = logger;
    }

    public IEnumerable<string> GetToolStrings() =>
        Enum.GetNames<ToolSignature>().ToList(); 

    public Dictionary<ToolSignature, ToolMetadata> GetRequiredTools() =>
        _requiredToolMap;

    public Dictionary<ToolSignature, ToolMetadata> GetOptionalTools() =>
        _optionalToolMap;

    public async ValueTask<ToolResponse> InvokeToolAsync(int toolId, ToolParameters? toolParameters = null) { 
        ToolSignature? signature = (Enum.IsDefined(typeof(ToolSignature), toolId))
            ? (ToolSignature)toolId
            : null;
        if(signature == null) {
            _logger.LogError($"\n{DateTime.Now}: Failed to query for tool usage, response was null.");
            return toolFailure;
        }

        ToolMetadata metaSignature = _optionalToolMap[signature.Value];
        return await metaSignature.CallTool();
    }

}
