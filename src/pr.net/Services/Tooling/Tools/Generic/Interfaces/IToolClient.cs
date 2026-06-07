using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;

namespace pr.net.Tooling;

public interface IToolClient {

    ValueTask<ToolResponse> FetchFileTree(PullReviewCreatedEvent prEvent);

    ValueTask<ToolResponse> FetchFile(PullReviewCreatedEvent prEvent, string filePath);

}

