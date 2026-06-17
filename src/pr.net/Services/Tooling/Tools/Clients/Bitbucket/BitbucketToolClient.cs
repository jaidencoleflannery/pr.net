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

    public async ValueTask<ToolResponse> FetchFileTree(PullReviewCreatedEvent prEvent) =>
        await this.Fetch(prEvent);

    public async ValueTask<ToolResponse> FetchFile(PullReviewCreatedEvent prEvent, string path) =>
        await this.Fetch(prEvent, path);

    private async ValueTask<ToolResponse> Fetch(PullReviewCreatedEvent prEvent, string path = "") {
        if(prEvent is not BitbucketPullReviewCreatedEventDto request) {
           _logger.LogError($"{nameof(FetchFile)}: The type of event does not match the injected service, returning early.");
           return ToolFail();
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new(request); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(FetchFile)}: Provided event payload contained an invalid value, returning early.");
           return ToolFail();
        }

        // this needs to always use the branch sha to avoid routing issues.
        try {
            using(HttpRequestMessage message = new(
                HttpMethod.Get, 
                $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/{path}")
            ) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", 
                    await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent)
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

