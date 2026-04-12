using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

using pr.net.Services.Tokens;

using pr.net.Configurations.Repo;

namespace pr.net.Services.Validations;

public class WebhookValidator(
    ITokenService _tokenService,
    IOptions<RepoConfiguration> _configuration,
    ILogger<WebhookValidator> _logger
) : IWebhookValidator {
    
    // signature is a sha256 encoded {secret + body} payload, we encrypt the same thing and compare to validate.
    public async Task<bool> ValidateWebhookSecretAsync(string signature, string body) {
        if(string.IsNullOrWhiteSpace(body) || string.IsNullOrWhiteSpace(signature)) {
            _logger.LogError($"Body or signature provided was null in {nameof(ValidateWebhookSecretAsync)}");
            return false;
        }

        string? webhookSecret = await _tokenService.GetTokenAsync(Token.PR_NET_WEBHOOK_SECRET);
        if(string.IsNullOrWhiteSpace(webhookSecret)) {
            _logger.LogError($"Unable to fetch Webhook Secret from storage in {nameof(ValidateWebhookSecretAsync)}");
            return false;
        }

        try {
            byte[] secret = Encoding.UTF8.GetBytes(webhookSecret);
            using HMACSHA256 hmac = new(secret);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var localSignature = "sha256=" + Convert.ToHexString(hash).ToLowerInvariant();

            bool result = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(localSignature),
                Encoding.UTF8.GetBytes(signature)
            );

            return result;
        } catch {
            _logger.LogError($"Unexpected failure occurred attempting to compare hashed values.");
            return false;
        }
    }

    public async Task<bool> ValidateEventTypeAsync(string type) {
        List<string> validEventTypes = _configuration.Value.AcceptedEvents;
        if(validEventTypes.Count == 0) {
            _logger.LogError($"No event types could be fetched from configuration in {nameof(ValidateEventTypeAsync)}");
            return false;
        }

        if(validEventTypes.Contains(type))
            return true;
        else
            return false;
    }

}