using pr.net.Models.Incoming.Generic;

using pr.net.Tooling.Generic;

namespace pr.net.Tooling;

public interface IReadFileTreeTool : ITool {

    Task<(bool, string? fileTree)> ReadFileTree(PullReviewCreatedEvent prEvent); 

}

