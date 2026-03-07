namespace pr.net.Models;

public static class Providers {

    // O(1) lookup (TryParse is O(n))
    public static Provider? ValidateProvider(string? provider) => 
        ProviderMap.TryGetValue(provider ?? "", out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid provider - ensure configuration has provider set as:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<Provider>())
                    )
                )
            );

    public static string? GetUrl(Provider provider) =>
        UrlMap.TryGetValue(provider, out var url)
            ? url
            : throw new InvalidOperationException("Provider's url could not be found.");

    private static Dictionary<string, Provider> ProviderMap =
        Enum.GetValues<Provider>()
            .ToDictionary(p => p.ToString(), p => p);

    private static Dictionary<Provider, string> UrlMap = 
        new() {
            [Provider.Anthropic] = "https://api.anthropic.com/v1/messages",
            [Provider.OpenAi] = "https://api.openai.com/v1/responses"
        };

    public enum Provider {
        Anthropic,
        OpenAi,
        Google,
        None
    }

}