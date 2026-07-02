using pr.net.Models.Incoming.Generic;
using pr.net.Models.Outbound.Generic;
using pr.net.Models.Tooling;

namespace pr.net.Services.Tooling;

public interface IToolClient {

    ValueTask<ToolResponse> FetchFileTree(ToolParameters parameters);

    ValueTask<ToolResponse> FetchFile(ToolParameters parameters);

}

