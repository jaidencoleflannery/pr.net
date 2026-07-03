using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Models.Tooling;

public class ToolParameters(
    int toolId,
    IEnumerable<string?> toolInput,
    PullReviewCreatedEvent? prEvent,
    DiffSection? diffSection
 ) {

    public int ToolId { get; set; } = toolId;

    public IEnumerable<string?> ToolInput { get; set; } = toolInput;

    public PullReviewCreatedEvent? PrEvent { get; set; } = prEvent;

    public DiffSection? DiffSections { get; set; } = diffSection;

}
