namespace pr.net.Services.Tokens;

public interface ITokenService {

    /// <summary>Retrieves the specified type of access token.</summary>
    /// <returns>The specified type of access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not configured.</exception>
    /// <remarks>Token is sourced from environment. Ensure the specified environment variable is set before calling.</remarks> 
    ValueTask<string> GetTokenAsync(Token type);

}