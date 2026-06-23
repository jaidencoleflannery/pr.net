using pr.net.Models.Outbound.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Generic;

namespace pr.net.Services.Tooling.Generic;

public interface ITool {

    ValueTask<ToolResponse> InvokeTool(PullReviewCreatedMetadata metadata, DiffSection[]? diffSections = null);

    ToolMetadata GetToolMetadata();

}

