namespace pr.net.Services.Tokens;

public interface ICachedToken {
    
    /// <summary>Retrieves a token from the cache.</summary>
    /// <returns>The access token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the token is not cached or configured.</exception>
    /// <remarks>Ensure the environment variable PR_NET_{type}_TOKEN is set before calling.</remarks> 
    ValueTask<string> GetAsync();

}