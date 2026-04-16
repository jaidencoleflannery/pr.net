using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

using pr.net.Services.Tokens;

using pr.net.Configurations.Repo;

using static pr.net.Models.Enums.RepoProviders;
using static pr.net.Models.Enums.Events;

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

    public bool ValidateEventType(string type, RepoProvider provider) {
        List<string> configuredEventTypes = _configuration.Value.AcceptedEvents;
        List<Event> validatedEventTypes = new();
        // build a list of configured event types.
        foreach(string line in configuredEventTypes) {
            Event result = StringToEvent(line, provider);
            if(result != Event.None)
                validatedEventTypes.Add(result);
        }
        if(validatedEventTypes.Count <= 0) {
            _logger.LogError($"No configured event types match stored values in {nameof(ValidateEventType)}");
            return false;
        }

        // compare provided event against our configured list of event types.
        Event eventInstance = StringToEvent(type, provider);
        if(validatedEventTypes.Contains(eventInstance))
            return true;
        else
            return false;
    }

    public bool ValidateUser(string id) {
        if(_configuration.Value.Users == null || _configuration.Value.Users?.AuthorizedUsers == null) {
            _logger.LogError($"No authorized users could be fetched from configuration in {nameof(ValidateUser)}");
            return false;
        }

        List<string> validUsers = _configuration.Value.Users?.AuthorizedUsers!; // this is a list of approved user's ids, not usernames.
        if(validUsers.Contains(id))
            return true;
        else
            return false;
    }

}