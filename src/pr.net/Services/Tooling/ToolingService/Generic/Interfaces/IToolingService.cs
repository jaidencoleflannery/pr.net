using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Enums;

namespace pr.net.Services.Tooling;

public interface IToolingService {

    IEnumerable<string> GetToolStrings();

    Dictionary<ToolSignature, ToolMetadata> GetRequiredTools();

    Dictionary<ToolSignature, ToolMetadata> GetOptionalTools();

    ValueTask<ToolResponse> InvokeToolAsync(ToolParameters parameters);

}
