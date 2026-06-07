using pr.net.Tooling;

using pr.net.Models.Enums;
using pr.net.Models.Tooling;

using static pr.net.Models.Enums.ToolSignature;

namespace pr.net.Services.Tooling;

public class ToolChainService(
        IReadFileTreeTool _readFileTreeTool
    ) : IToolChainService {

    // all added tools need to be populated within the ToolSignature enum and have a populated ToolMetadata instance to be added to the mapping.
    private static Dictionary<ToolSignature, ToolMetadata> _toolMap = new();

    public static bool Initialize() {
        _toolMap = new() {

            [ReadFileTree] = new ToolMetadata(
                Name = ReadFileTree.ToString(), 
                Description = "Get repository directory tree.",
                ToolPointer = _readFileTreeTool.ReadFileTree
            ),

            [ReadFile] = new ToolMetadata(
                Name = ReadFile.ToString(),
                Description = "Read a specified file in the repository",
                ToolPointer = /* delegate here */
            )

        };
        return true;
    }

    public IEnumerable<string> GetToolStrings() =>
        Enum.GetNames<ToolSignature>().ToList();

    public Dictionary<ToolSignature, ToolMetadata> GetTools() =>
        _toolMap;

}

