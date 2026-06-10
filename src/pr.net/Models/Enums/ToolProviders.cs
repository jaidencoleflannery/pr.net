namespace pr.net.Models.Enums;

public static class ToolProviders {

    public ToolProvider ValidateToolProvider(string? provider) =>
        ToolMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider)
            ? foundProvider
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Tool Provider - ensure configuration has Tool Provider set as one of the following:",
                    string.Join(Environment.Newline,
                        string.Join("    ", Enum.GetValues<HostProvider>())
                    )
                )
        );

    public static readonly Dictionary<string, ToolProvider> ToolMap =
        Enum.GetValues<ToolProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum ToolProvider {
        Amazon,
        Environment,
        None,
    }
}

