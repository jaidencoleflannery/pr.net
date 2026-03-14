namespace pr.net.Services.Tokens;

public enum Token { PR_NET_REPO_TOKEN, PR_NET_CHAT_TOKEN }

public static class TokenMap {
    // since we know what Tokens holds, this gives us O(1) lookup time instead of waiting for tostring to lookup the name
    private static Dictionary<Token, string> _map = Enum.GetValues<Token>().ToDictionary(
        token => token,
        token => token.ToString()
    );

    public static string GetString(this Token token) =>
        _map[token];

}