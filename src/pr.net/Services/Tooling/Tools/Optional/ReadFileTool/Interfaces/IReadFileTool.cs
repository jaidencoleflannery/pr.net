using pr.net.Models.Incoming.Generic;

using pr.net.Tooling.Generic;

namespace pr.net.Tooling;

public interface IReadFileTool : ITool {

    Task<(bool Success, string? Result)> ReadFile(PullReviewCreatedEvent prEvent, string filePath); 

}

