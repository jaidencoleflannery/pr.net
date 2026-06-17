using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

using pr.net.Services.Tooling.Generic;

namespace pr.net.Services.Tooling;

public interface IReadFileTreeTool : ITool {

    Task<ToolResponse> ReadFileTree(PullReviewCreatedEvent prEvent);

}

