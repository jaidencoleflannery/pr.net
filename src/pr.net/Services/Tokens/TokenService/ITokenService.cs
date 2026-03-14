namespace pr.net.Services.Tokens;

public interface ITokenService {

    /// <summary>Retrieves the repository provider's access token.</summary>
    /// <returns>The repository access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not configured.</exception>
    /// <remarks>Token is sourced from environment. Ensure the environment variable PR_NET_REPO_TOKEN is set before calling.</remarks> 
    ValueTask<string> GetRepoTokenAsync();

    /// <summary>Retrieves the AI provider's access token.</summary>
    /// <returns>The repository access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not configured.</exception>
    /// <remarks>Token is sourced from environment. Ensure the environment variable PR_NET_CHAT_TOKEN is set before calling.</remarks>
    ValueTask<string> GetChatTokenAsync();

}