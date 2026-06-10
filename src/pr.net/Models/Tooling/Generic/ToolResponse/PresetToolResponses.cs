namespace pr.net.Models.Tooling;

public static class PresetToolResponses {

    private static ToolResponse _fail = new() {
        Success = false,
        Result = null
    };

    // function call incase we want to extend tool failure logic in the future.
    public static ToolResponse ToolFail() =>
        _fail;

}

