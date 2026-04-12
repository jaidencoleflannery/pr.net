namespace pr.net.Services.Validations;

public interface IWebhookValidator {

    Task<bool> ValidateWebhookSecretAsync(string signature, string body);

    Task<bool> ValidateEventTypeAsync(string type);

}