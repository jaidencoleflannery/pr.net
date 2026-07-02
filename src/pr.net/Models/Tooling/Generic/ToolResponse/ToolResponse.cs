namespace pr.net.Models.Tooling;

public struct ToolResponse {
    public bool Success { get; init; } = false;
    public string ToolName { get; init; } = string.Empty; 
    public string[] Result { get; init; } = [];
    
    public ToolResponse(bool success, string toolName, string[] result) {
        this.Success = success;
        this.ToolName = toolName;
        this.Result = result;
    }

}

