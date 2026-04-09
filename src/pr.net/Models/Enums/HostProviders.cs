namespace pr.net.Models.Enums;

public static class HostProviders {

    // O(1) lookup (TryParse is O(n)).
    public static HostProvider ValidateHostProvider(string? provider) => 
        HostMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Host Provider - ensure configuration has Host Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<HostProvider>())
                    )
                )
            );

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow.
    private static readonly Dictionary<string, HostProvider> HostMap =
        Enum.GetValues<HostProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum HostProvider {
        Amazon, 
        Environment,
        None
    }

}