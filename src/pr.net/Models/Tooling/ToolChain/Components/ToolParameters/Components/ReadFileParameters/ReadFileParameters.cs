using pr.net.Models.Incoming.Generic;

namespace pr.net.Models.Tooling;

public class ReadFileParameters : ToolParameters {
    public PullReviewCreatedEvent PrEvent { get; set; }
    public string FilePath { get; set; }

    public ReadFileParameters(PullReviewCreatedEvent input, string filePath) {
        this.PrEvent = input;
        this.FilePath = filePath;
    }
}

