using pr.net.Services.Tooling;

using pr.net.Models.Enums;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ToolSignature;

namespace pr.net.Services.Tooling.Environment;

public class EnvironmentToolChainService(
        IReadFileTreeTool _readFileTreeTool,
        IReadFileTool _readFileTool
    ) : IToolChainService {

    // all added tools need to be populated within the ToolSignature enum and have a populated ToolMetadata instance to be added to the mapping.
    private static Dictionary<ToolSignature, ToolMetadata> _requiredToolMap = [];
    private static Dictionary<ToolSignature, ToolMetadata> _optionalToolMap = [];

    public bool Initialize() {
        _requiredToolMap = new() { };
        _optionalToolMap = new() {

            [ReadFileTree] = new ToolMetadata {
                Name = ReadFileTree.ToString(), 
                Description = "Get repository directory tree.",
                ToolPointer = _readFileTreeTool.InvokeTool
            },

            [ReadFile] = new ToolMetadata {
                Name = ReadFile.ToString(),
                Description = $"Read a specified file from the repository, can only be used after {nameof(ReadFileTree)} has been invoked.",
                IsChild = true,
                ParentPointer = _readFileTreeTool.InvokeTool,
                ToolPointer = _readFileTool.InvokeTool
            }

        };
        return true;
    }

    public IEnumerable<string> GetToolStrings() =>
        Enum.GetNames<ToolSignature>().ToList();

    public Dictionary<ToolSignature, ToolMetadata> GetRequiredTools() =>
        _requiredToolMap;

    public Dictionary<ToolSignature, ToolMetadata> GetOptionalTools() =>
        _optionalToolMap;

}

