using pr.net.Models.Incoming;
using pr.net.Models.Tooling;

namespace pr.net.Models.Generic;

public class DiffSection(string path, string contents) {

    public string Path { get; set; } = path;

    public string Contents { get; set; } = contents;

    public List<ToolResponse> Context { get; set; } = [];

    public ChatResponse? Review { get; set; } = null; 

}

