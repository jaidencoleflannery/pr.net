using pr.net.Models.Tooling;
using pr.net.Models.Enums;

using static pr.net.Models.Enums.ToolSignature;

namespace pr.net.Services.Tooling;

public class ToolChainService : IToolChainService {

    // all added tools need to be populated within the ToolSignature enum and have a populated Tool instance to be added to the mapping.
    private static Dictionary<ToolSignature, Tool> _toolMap = new();

    private ToolChainService() { }

    public static bool Initialize() {
        _toolMap = new() {

            [ReadFileTree] = new Tool(
                ReadFileTree.ToString(), 
                /* delegate here */
            ),

            [ReadFile] = new Tool(
                ReadFile.ToString(), 
                /* delegate here */
            )

        };
        return true;
    }

    public IEnumerable<string> GetToolStrings() =>
        Enum.GetNames<ToolSignature>().ToList();

    public Dictionary<ToolSignature, Tool> GetTools() =>
        _toolMap;

}

