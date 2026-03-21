namespace pr.net.Models.Enums;

public static class RepoProviders {

    // O(1) lookup (TryParse is O(n))
    public static RepoProvider ValidateRepoProvider(string? provider) => 
        ProviderMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Repo Provider - ensure configuration has Repo Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<RepoProvider>())
                    )
                )
            );

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow
    private static readonly Dictionary<string, RepoProvider> ProviderMap =
        Enum.GetValues<RepoProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    public enum RepoProvider {
        Github,
        Bitbucket,
        Gitlab,
        None
    }

}