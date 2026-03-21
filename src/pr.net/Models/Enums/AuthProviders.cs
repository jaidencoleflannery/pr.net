namespace pr.net.Models.Enums;

public static class AuthProviders {

    // O(1) lookup (TryParse is O(n))
    public static AuthProvider ValidateAuthProvider(string? provider) => 
        ProviderMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Auth Provider - ensure configuration has Auth Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<AuthProvider>())
                    )
                )
            );

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow
    private static readonly Dictionary<string, AuthProvider> ProviderMap =
        Enum.GetValues<AuthProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum AuthProvider {
        Environment, 
        None
    }

}