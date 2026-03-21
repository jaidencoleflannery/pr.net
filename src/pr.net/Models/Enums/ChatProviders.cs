namespace pr.net.Models.Enums;

public static class ChatProviders {

    // O(1) lookup (TryParse is O(n))
    public static ChatProvider ValidateChatProvider(string? provider) => 
        ProviderMap.TryGetValue((provider ?? "").ToLower(), out var foundProvider) 
            ? foundProvider 
            : throw new InvalidOperationException(
                string.Join(Environment.NewLine, "Invalid Chat Provider - ensure configuration has Repo Provider set as one of the following:",
                    string.Join(Environment.NewLine, 
                        string.Join("    ", Enum.GetValues<ChatProvider>())
                    )
                )
            );

    public static string? GetUrl(ChatProvider provider) =>
        UrlMap.TryGetValue(provider, out var url)
            ? url
            : throw new InvalidOperationException("Chat Provider's url could not be found.");

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow
    private static readonly Dictionary<string, ChatProvider> ProviderMap =
        Enum.GetValues<ChatProvider>()
            .ToDictionary(p => p.ToString().ToLower(), p => p);

    private static readonly Dictionary<ChatProvider, string> UrlMap = 
        new() {
            [ChatProvider.Anthropic] = "https://api.anthropic.com/v1/messages",
            [ChatProvider.OpenAi] = "https://api.openai.com/v1/responses"
        };

    public enum ChatProvider {
        Anthropic,
        OpenAi,
        Google,
        None
    }

}