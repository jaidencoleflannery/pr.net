namespace pr.net.Services.Tokens;

using pr.net.Models.Incoming.Generic;

/// <summary>An access token factory that handles token expiration.</summary>
/// <remarks>If a token does not expire, skew towards injecting it directly into the singleton instead of using this service to avoid performance implications.</remarks> 
public interface ITokenService {

    /// <summary>Retrieves the specified type of access token.</summary>
    /// <returns>The specified type of access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not configured.</exception>
    /// <remarks>Token is sourced from environment. Ensure the specified environment variable is set before calling.</remarks>
    ValueTask<string?> GetTokenAsync(Token type, PullReviewCreatedEvent? prEvent = null);

}
