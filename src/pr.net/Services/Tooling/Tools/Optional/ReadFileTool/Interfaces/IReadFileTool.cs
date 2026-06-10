using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

using pr.net.Services.Tooling.Generic;

namespace pr.net.Services.Tooling;

public interface IReadFileTool : ITool {

    Task<ToolResponse> ReadFile(PullReviewCreatedEvent prEvent, string filePath); 

}

