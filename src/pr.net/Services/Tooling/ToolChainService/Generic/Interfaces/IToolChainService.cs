using pr.net.Models.Tooling;
using pr.net.Models.Enums;

namespace pr.net.Services.Tooling;

public interface IToolChainService {

    bool Initialize();

    IEnumerable<string> GetToolStrings();

    Dictionary<ToolSignature, ToolMetadata> GetRequiredTools();

    Dictionary<ToolSignature, ToolMetadata> GetOptionalTools();

}

