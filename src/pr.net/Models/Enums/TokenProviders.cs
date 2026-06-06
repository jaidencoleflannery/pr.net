namespace pr.net.Models.Enums;

public static class TokenProviders {

    // O(1) lookup (TryParse is O(n)).
    public static TokenProvider ValidateTokenProvider(string? provider) => 
        TokenMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Token Provider - ensure configuration has Token Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<TokenProvider>())
                    )
                )
            );

    private static readonly Dictionary<string, TokenProvider> TokenMap =
        Enum.GetValues<TokenProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum TokenProvider {
        AmazonSecretsManager,
        Environment,
        None
    }

}
