using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Services.Validations;

public interface IWebhookValidator {

    Task<bool> ValidateWebhookSecretAsync(string signature, string body);

    bool ValidateEventType(string type, RepoProvider provider);

    bool ValidateUser(string username);

}