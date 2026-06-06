namespace pr.net.Models.Tooling;

public struct ToolResponse {
    public bool Success { get; init; } = false;
    public List<string> Value { get; init; } = new() {{ string.Empty }};

    public ToolResponse(bool success, IEnumerable<string> value) {
        this.Success = success;
        this.Value = (List<string>)value;
    }
}

