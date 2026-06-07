using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

using pr.net.Tooling.Generic;

namespace pr.net.Tooling;

public interface IReadFileTool : ITool {

    Task<ToolResponse> ReadFile(PullReviewCreatedEvent prEvent, string filePath); 

}

