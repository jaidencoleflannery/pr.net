using System.Text.Json;

using Microsoft.Extensions.Options;

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

using pr.net.Configurations.Host;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tokens;

namespace pr.net.Services.Tokens;

public class AmazonTokenProvider(
    IOptions<HostConfiguration> _hostConfiguration,
    ILogger<AmazonTokenProvider> _logger,
    IAmazonSecretsManager _amazonSecretManager
) : ITokenProvider { 

    public async ValueTask<string?> FetchAsync(Token target, PullReviewCreatedEvent? prEvent = null) =>
        await this.FetchAsync(target);

    public async ValueTask<string?> FetchAsync(Token target) {

        if(_hostConfiguration.Value?.Amazon == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Secrets Manager path could not be fetched from configuration in {nameof(FetchAsync)}. ]\n");
            return null;
        }

        string? path = _hostConfiguration.Value.Amazon!.SecretsManagerPath;

        if(string.IsNullOrWhiteSpace(path)) {
            _logger.LogError($"\n{DateTime.Now}: [ Secrets Manager path could not be fetched from configuration in {nameof(FetchAsync)}. ]\n");
            return null;
        }

        GetSecretValueResponse response = await _amazonSecretManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = path });

        string? jsonString = response?.SecretString;
        if(jsonString == null) {
            _logger.LogError($"\n{DateTime.Now}: [ JSON response from Amazon Secrets Manager is either null or in an invalid format in {nameof(FetchAsync)}. ]\n");
            return null;
        }

        var json = JsonSerializer.Deserialize<AmazonKeySet>(jsonString);
        if(json == null) {
            _logger.LogError($"\n{DateTime.Now}: [ JSON response from Amazon Secrets Manager could not be parsed in {nameof(FetchAsync)}. ]\n");
            return null;
        }

        string? key = null;
        if(target == Token.PR_NET_REPO_TOKEN)
            key = json!.PR_NET_REPO_TOKEN;
        else if(target == Token.PR_NET_CHAT_TOKEN) 
            key = json!.PR_NET_CHAT_TOKEN;
        if(key == null) {
            _logger.LogError($"\n{DateTime.Now}: [ Token target could not be pulled from Amazon Secrets Manager response in {nameof(FetchAsync)}. ]\n");
            return null;
        }

        return key;
    }

}