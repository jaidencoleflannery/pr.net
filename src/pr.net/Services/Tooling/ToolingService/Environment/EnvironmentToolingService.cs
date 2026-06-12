using pr.net.Models.Enums;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ToolSignature;

namespace pr.net.Services.Tooling.Environment;

public class EnvironmentToolingService : IToolingService {

    // all added tools need to be populated within the ToolSignature enum and have a populated ToolMetadata instance to be added to the mapping.
    private static Dictionary<ToolSignature, ToolMetadata> _requiredToolMap = [];
    private static Dictionary<ToolSignature, ToolMetadata> _optionalToolMap = [];

    private IReadFileTreeTool _readFileTreeTool;
    private IReadFileTool _readFileTool;

    public EnvironmentToolingService(
        IReadFileTreeTool readFileTreeTool, 
        IReadFileTool readFileTool
    ) {
        _readFileTreeTool = readFileTreeTool;
        _readFileTool = readFileTool;

        _requiredToolMap = new() { };
        _optionalToolMap = new() {
            [ReadFileTree] = _readFileTreeTool.GetToolMetadata(),
            [ReadFile] = _readFileTool.GetToolMetadata() 
        };
    }

    public IEnumerable<string> GetToolStrings() =>
        Enum.GetNames<ToolSignature>().ToList();

    public Dictionary<ToolSignature, ToolMetadata> GetRequiredTools() =>
        _requiredToolMap;

    public Dictionary<ToolSignature, ToolMetadata> GetOptionalTools() =>
        _optionalToolMap;

}

