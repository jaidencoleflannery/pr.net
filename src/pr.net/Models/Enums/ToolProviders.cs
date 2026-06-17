namespace pr.net.Models.Enums;

public static class ToolingProviders {

    public static ToolingProvider ValidateToolingProvider(string? provider) =>
        ToolMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider)
            ? foundProvider
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Tool Provider - ensure configuration has Tool Provider set as one of the following:",
                    string.Join(Environment.NewLine,
                        string.Join("    ", Enum.GetValues<ToolingProvider>())
                    )
                )
        );

    public static readonly Dictionary<string, ToolingProvider> ToolMap =
        Enum.GetValues<ToolingProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum ToolingProvider {
        Amazon,
        Environment,
        None,
    }
}

