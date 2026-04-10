using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

/// <summary>A handler for tokens; manages token logic such as fetching, encryption and transformation.</summary>
public interface ITokenHandler {
    /// <summary>Retrieves a token from the implemented configuration or environment.</summary>
    /// <returns>The access token as a string.</returns>
    /// <remarks>Ensure the environment variable PR_NET_{type}_TOKEN is set before calling.</remarks> 
    ValueTask<string?> FetchAsync(PullReviewCreatedEvent? prEvent = null);
}