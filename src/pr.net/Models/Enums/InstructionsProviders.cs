namespace pr.net.Models.Enums;

public static class InstructionsProviders {

    // O(1) lookup (TryParse is O(n))
    public static InstructionsProvider ValidateInstructionsProvider(string? provider) => 
        ProviderMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Instructions Provider - ensure configuration has Instructions Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<InstructionsProvider>())
                    )
                )
            );

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow
    private static readonly Dictionary<string, InstructionsProvider> ProviderMap =
        Enum.GetValues<InstructionsProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum InstructionsProvider {
        Environment, 
        None
    }

}