namespace pr.net.Services.Tokens;

public enum Token { 
    PR_NET_REPO_TOKEN, 
    PR_NET_CHAT_TOKEN 
}

public static class Tokens { 

    // since we know what Tokens holds, this gives us O(1) lookup time instead of waiting for tostring to lookup the name
    private static Dictionary<Token, string> _tokenToStringMap = Enum.GetValues<Token>().ToDictionary(
        token => token,
        token => token.ToString().ToLower()
    );

    // inverse lookup
    private static Dictionary<string, Token> _stringToTokenMap = Enum.GetValues<Token>().ToDictionary(
        token => token.ToString().ToLower(),
        token => token
    );

    public static string GetString(this Token token) =>
        _tokenToStringMap[token];

    public static Token GetToken(string target) =>
        _stringToTokenMap[target];

}