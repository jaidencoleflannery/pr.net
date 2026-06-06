namespace pr.net.Models.Tooling;

public struct Tool {
    private bool _invoked { get; init; } = false;
    private Func<string[], Task<ToolResponse>> _toolPointer { get; init; }

    public string Name { get; init; }
    public string Description { get; init; } 

    public Tool(
        string name, 
        string description, 
        Func<string[], Task<ToolResponse>> toolPointer
    ) {
        Name = name;
        Description = description;
        _toolPointer = toolPointer;
    }

    public async Task<ToolResponse> CallTool(string[] input) =>
        await _toolPointer(input);
}

