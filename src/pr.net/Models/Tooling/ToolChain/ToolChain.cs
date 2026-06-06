namespace pr.net.Models.Tooling;

// parent container for child tools.
public class ToolChain {

    IEnumerable<Func<ToolSignature[], ToolResponse>> RequiredTools = [];
    IEnumerable<Func<ToolSignature[], ToolResponse>> OptionalTools = [];

}

