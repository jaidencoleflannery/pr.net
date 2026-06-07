namespace pr.net.Models.Tooling;

public struct ToolResponse {
    public bool Success { get; init; } = false;
    public ToolValue? Result { get; init; }

    public ToolResponse(bool success, ToolValue result) {
        this.Success = success;
        this.Result = (ToolValue)result;
    }
}

