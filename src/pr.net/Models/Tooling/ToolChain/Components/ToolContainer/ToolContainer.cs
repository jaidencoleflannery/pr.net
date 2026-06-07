namespace pr.net.Models.Tooling;

public struct ToolMetadata {
    private bool _invoked { get; init; } = false; 

    public string Name { get; init; }
    public string Description { get; init; } 
    public Func<string[], Task<ToolResponse>> ToolPointer { get; init; }

    public ToolMetadata(
        string name, 
        string description, 
        Func<string[], Task<ToolResponse>> toolPointer
    ) {
        Name = name;
        Description = description;
        ToolPointer = toolPointer;
    }

    public async Task<ToolResponse> CallTool(string[] input) =>
        await ToolPointer(input);
}

