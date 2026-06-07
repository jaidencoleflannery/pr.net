using pr.net.Models.Tooling;

namespace pr.net.Tooling.Generic;

public interface ITool {

    ValueTask<ToolResponse> InvokeTool(ToolParameters parameters); 

}

