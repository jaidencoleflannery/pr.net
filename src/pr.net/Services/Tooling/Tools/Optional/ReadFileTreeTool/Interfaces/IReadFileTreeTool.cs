using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

using pr.net.Tooling.Generic;

namespace pr.net.Tooling;

public interface IReadFileTreeTool : ITool {

    Task<ToolResponse> ReadFileTree(PullReviewCreatedEvent prEvent); 

}

