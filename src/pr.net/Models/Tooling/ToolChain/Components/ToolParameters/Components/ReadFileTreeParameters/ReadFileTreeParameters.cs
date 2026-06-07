using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Tooling;

public class ReadFileTreeParameters : ToolParameters {
    PullReviewCreatedEvent prEvent;

    public ReadFileTreeParameters(PullReviewCreatedEvent input) {
        this.prEvent = input;
    }
}

