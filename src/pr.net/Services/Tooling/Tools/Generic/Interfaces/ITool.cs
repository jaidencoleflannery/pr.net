using pr.net.Models.Tooling;

namespace pr.net.Services.Tooling.Generic;

public interface ITool {

    ValueTask<ToolResponse> InvokeTool(ToolParameters parameters);

}

