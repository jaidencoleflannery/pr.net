using pr.net.Services.Tokens;

namespace pr.net.Services.Validations;

public class BitbucketValidator(
    ITokenService _tokenService
) : IValidator {
    
    public bool ValidateType(string eventType) =>
        eventType is "pullrequest:created"; // make this dynamic per configuration.

    public async Task<bool> ValidateWebhookSecretAsync(string signature, string? _ = null) =>
        signature == await _tokenService.GetTokenAsync(Token.PR_NET_WEBHOOK_SECRET);

}