using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Models.Tooling;

public class ToolParameters(
    uint toolId,
    IEnumerable<string?> toolInput,
    PullReviewCreatedEvent? prEvent,
    IEnumerable<DiffSection?> diffSections
 ) {

    public uint ToolId { get; set; } = toolId;

    public IEnumerable<string?> ToolInput { get; set; } = toolInput;

    public PullReviewCreatedEvent? PrEvent { get; set; } = prEvent;

    public IEnumerable<DiffSection?> DiffSections { get; set; } = diffSections;

}
