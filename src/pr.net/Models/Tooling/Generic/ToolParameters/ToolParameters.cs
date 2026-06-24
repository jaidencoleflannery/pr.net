using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Models.Tooling;

public class ToolParameters {

    public uint ToolId { get; set; }

    public IEnumerable<string?> ToolInput { get; set; }

    public PullReviewCreatedEvent? PrEvent { get; set; } 

    public IEnumerable<DiffSection?> DiffSections { get; set; } 

    public ToolParameters(
        uint toolId, 
        IEnumerable<string?> toolInput, 
        PullReviewCreatedEvent? prEvent, 
        IEnumerable<DiffSection?> diffSections
    ) {
        this.ToolId = toolId;
        this.ToolInput = toolInput ?? [];
        this.PrEvent = prEvent;
        this.DiffSections = diffSections ?? [];
    } 
    
}

