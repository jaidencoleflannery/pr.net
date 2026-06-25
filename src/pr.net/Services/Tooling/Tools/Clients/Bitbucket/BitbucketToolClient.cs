using pr.net.Services.Tokens;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Bitbucket;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class BitbucketToolClient(
        HttpClient client,
        ITokenService _tokenService,
        ILogger<BitbucketToolClient> _logger
    ) : IToolClient {

    // this is obviously wrong.
    public async ValueTask<ToolResponse> FetchFileTree(ToolParameters parameters) =>
        await this.Fetch(parameters);

    public async ValueTask<ToolResponse> FetchFile(ToolParameters parameters) =>
        await this.Fetch(parameters);

    private async ValueTask<ToolResponse> Fetch(ToolParameters parameters) {
        if(parameters.PrEvent is null or not BitbucketPullReviewCreatedEventDto) {
           _logger.LogError($"{nameof(Fetch)}: The type of event does not match the injected service, or is invalid, short circuiting.");
           return ToolFail();
        }

        if(parameters.ToolInput.Count() != 1) {
            _logger.LogError($"{nameof(Fetch)}: Input for Fetch was invalid.");
           return ToolFail();
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new((BitbucketPullReviewCreatedEventDto)parameters.PrEvent); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(Fetch)}: Provided event payload contained an invalid value, short circuiting.");
           return ToolFail();
        }

        string? path = parameters.ToolInput.First();
        if(string.IsNullOrWhiteSpace(path)) {
            _logger.LogError($"{nameof(Fetch)}: Path was invalid.");
           return ToolFail();
        }
// 
        // this needs to always use the branch sha to avoid routing issues.
        try {
            using(HttpRequestMessage message = new(
                HttpMethod.Get, 
                $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/{path}")
            ) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", 
                    await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, parameters.PrEvent)
                );
                
                using HttpResponseMessage response = await client.SendAsync(message);
                // TODO: this is the entire response, need to get the actual response out of it.
                string content = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode) {
                    if(string.IsNullOrWhiteSpace(content))
                        // this is within the try/catch, don't throw upchain in tools.
                        throw new InvalidOperationException($"{nameof(FetchFile)}: Response content was invalid, failed to fetch.");

                    return new ToolResponse {
                        Success = true,
                        Result = [content]
                    };
                }
                _logger.LogError($"{nameof(FetchFile)}: Fetch request was unsuccessful.");
                return ToolFail();
            }
        } catch (Exception error) {
            _logger.LogError($"{nameof(FetchFile)}: Failed to fetch file contents. Error encountered: {error}.");
            return ToolFail();
        }
    }

}

