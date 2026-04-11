namespace pr.net.Services.Validations;

public interface IValidator {

    bool ValidateType(string eventType);

    Task<bool> ValidateWebhookSecretAsync(string signature, string? body = null);

}