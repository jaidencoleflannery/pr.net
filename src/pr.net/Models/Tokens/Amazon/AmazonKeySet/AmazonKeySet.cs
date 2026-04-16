namespace pr.net.Services.Tokens;

public class AmazonKeySet {

    private Dictionary<Token, string> _keyHash = Enum.GetValues<Token>().ToDictionary(token => token, token => token.ToString() ?? string.Empty);

    public string? GetToken(Token type) {
        switch(type) {
            case Token.PR_NET_REPO_TOKEN:
                return PR_NET_REPO_TOKEN;

            case Token.PR_NET_CHAT_TOKEN:
                return PR_NET_CHAT_TOKEN;

            case Token.PR_NET_WEBHOOK_SECRET:
                return PR_NET_WEBHOOK_SECRET;

            default:
                return null;
        }
    }

    public string PR_NET_REPO_TOKEN { get => _keyHash[Token.PR_NET_REPO_TOKEN]; set => _keyHash[Token.PR_NET_REPO_TOKEN] = value; }

    public string PR_NET_CHAT_TOKEN { get => _keyHash[Token.PR_NET_CHAT_TOKEN]; set => _keyHash[Token.PR_NET_CHAT_TOKEN] = value; }

    public string PR_NET_WEBHOOK_SECRET { get => _keyHash[Token.PR_NET_WEBHOOK_SECRET]; set => _keyHash[Token.PR_NET_WEBHOOK_SECRET] = value; }

}