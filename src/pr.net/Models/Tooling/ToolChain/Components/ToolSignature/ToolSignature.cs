namespace pr.net.Models.Tooling;

public struct ToolSignature {
    public string Name;    
    public Func<string[], Task<ToolResponse>> ToolPointer;

    public ToolSignature(string name, Func<string[], Task<ToolResponse>> toolPointer) {
        Name = name;
        ToolPointer = toolPointer;
    }

    public async Task<ToolResponse> CallTool(string[] input) =>
        await ToolPointer(input);
}

