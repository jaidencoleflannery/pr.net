namespace pr.net.Models.Tooling;

public struct ToolResponse {
    public bool Success { get; init; } = false;
    public ToolValue? Value { get; init; }

    public ToolResponse(bool success, ToolValue value) {
        this.Success = success;
        this.Value = (ToolValue)value;
    }
}

