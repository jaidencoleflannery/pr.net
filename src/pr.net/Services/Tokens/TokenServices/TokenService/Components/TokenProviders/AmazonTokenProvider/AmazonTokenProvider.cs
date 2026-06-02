using System.Text.Json;

using Microsoft.Extensions.Options;

using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

using pr.net.Configurations.Host;

using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class AmazonTokenProvider(
    IOptions<HostConfiguration> _hostConfiguration,
    IAmazonSecretsManager _amazonSecretManager
) : ITokenProvider { 

    public async ValueTask<string?> FetchAsync(Token target, PullReviewCreatedEvent? prEvent = null) =>
        await this.FetchAsync(target);

    public async ValueTask<string?> FetchAsync(Token target) {

        if(_hostConfiguration.Value?.Amazon == null)
            throw new InvalidOperationException($"Secrets Manager path could not be fetched from configuration in {nameof(FetchAsync)}.");

        string? path = _hostConfiguration.Value.Amazon!.SecretsManagerPath;
        if(string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException($"Secrets Manager path could not be fetched from configuration in {nameof(FetchAsync)}.");

        GetSecretValueResponse response = await _amazonSecretManager.GetSecretValueAsync(new GetSecretValueRequest { SecretId = path });

        string? jsonString = response?.SecretString;
        if(jsonString == null)
            throw new InvalidOperationException($"JSON response from Amazon Secrets Manager is either null or in an invalid format in {nameof(FetchAsync)}.");

        var json = JsonSerializer.Deserialize<AmazonKeySet>(jsonString);
        if(json == null)
            throw new InvalidOperationException($"JSON response from Amazon Secrets Manager could not be parsed in {nameof(FetchAsync)}."); 

        string? key = json.GetToken(target);
        if(key == null)
            throw new InvalidOperationException($"Token target {key} could not be pulled from Amazon Secrets Manager response in {nameof(FetchAsync)}."); 

        return key;
    }

}
