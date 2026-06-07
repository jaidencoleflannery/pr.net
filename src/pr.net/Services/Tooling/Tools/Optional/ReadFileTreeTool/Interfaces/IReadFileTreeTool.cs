using pr.net.Models.Incoming.Generic;

using pr.net.Tooling.Generic;

namespace pr.net.Tooling;

public interface IReadFileTreeTool : ITool {

    Task<(bool Success, string? Result)> ReadFileTree(PullReviewCreatedEvent prEvent); 

}

