using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Tooling;

public class ReadFileTreeParameters : ToolParameters {
    public PullReviewCreatedEvent prEvent { get; set; }

    public ReadFileTreeParameters(PullReviewCreatedEvent input) {
        this.prEvent = input;
    }
}

