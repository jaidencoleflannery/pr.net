using System.Text;
using System.Security.Cryptography;

using pr.net.Services.Tokens;

namespace pr.net.Services.Validations;

public class GithubValidator(
    ITokenService _tokenService
) : IValidator {
    
    public bool ValidateType(string eventType) =>
        eventType is "opened" or "reopened"; // make this dynamic per configuration.

    public async Task<bool> ValidateWebhookSecretAsync(string signature, string? body = null) {
        if(string.IsNullOrWhiteSpace(body))
            throw new InvalidOperationException($"Body provided was null in {nameof(ValidateWebhookSecretAsync)}");

        string? webhookSecret = await _tokenService.GetTokenAsync(Token.PR_NET_WEBHOOK_SECRET);
        if(string.IsNullOrWhiteSpace(webhookSecret))
            throw new InvalidOperationException($"Unable to fetch Webhook Secret from storage in {nameof(ValidateWebhookSecretAsync)}");

        byte[] secret = Encoding.UTF8.GetBytes(webhookSecret);
        using HMACSHA256 hmac = new(secret);
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
        var localSignature = "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(localSignature),
            Encoding.UTF8.GetBytes(signature)
        );
    }

}