namespace pr.net.Models.Tooling;

public struct ToolMetadata {

    public string Name { get; init; }
    public string Description { get; init; } 
    public bool IsChild { get; init; } = false; // if true, function can only be called after it's parent.
    public Func<ToolParameters?, ValueTask<ToolResponse>> ToolPointer { get; init; }

    public ToolMetadata(
        string name, 
        string description, 
        bool isChild,
        Func<ToolParameters?, ValueTask<ToolResponse>> toolPointer
    ) {
        this.Name = name;
        this.Description = description;
        this.IsChild = isChild;
        this.ToolPointer = toolPointer;
    }

    public async readonly Task<ToolResponse> CallTool(ToolParameters? input = null) =>
        await ToolPointer(input);
}

