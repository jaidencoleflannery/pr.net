using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public interface ITokenProvider {

    /// <summary>Retrieves a token from the implemented configuration or environment.</summary>
    /// <returns>The access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not configured.</exception>
    /// <remarks>Ensure the environment variable PR_NET_{type}_TOKEN is set before calling.</remarks> 
    ValueTask<string?> FetchAsync(Token target, PullReviewCreatedEvent? prEvent = null);

}