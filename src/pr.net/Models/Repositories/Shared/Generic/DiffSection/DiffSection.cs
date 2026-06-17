using pr.net.Models.Incoming;

namespace pr.net.Models.Generic;

public class DiffSection { 

    public string Path { get; set; } = string.Empty;

    public string Contents { get; set; } = string.Empty;

    public ChatResponse? Context { get; set; } = null;

    public ChatResponse? Review { get; set; } = null;

    public DiffSection(string path, string contents) {
        this.Path = path;
        this.Contents = contents;
    }

}

