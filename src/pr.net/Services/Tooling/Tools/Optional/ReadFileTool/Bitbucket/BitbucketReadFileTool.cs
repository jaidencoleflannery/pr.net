using pr.net.Services.Tokens;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Bitbucket;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Tooling;

public class BitbucketReadFileTool(
        HttpClient client, 
        ITokenService _tokenService,
        ILogger _logger
    ) : IReadFileTool {

    public async ValueTask<ToolResponse> InvokeTool(ToolParameters parameters) {
        if(parameters is not ReadFileParameters input
        || input.PrEvent == null
        || input.FilePath == null) {
            _logger.LogError($"{nameof(ReadFile)}: Failed to invoke tool, parameters given were invalid");
            return ToolFail();
        }

        (bool Success, string? Result) result = await ReadFile(input.PrEvent, input.FilePath);
        if(!result.Success
        || string.IsNullOrWhiteSpace(result.Result)) {
            _logger.LogError($"{nameof(ReadFile): Invocation of tool failed.}");
            return ToolFail();
        }

        return new ToolResponse {
            Success = result.Success,
            Value = new StringToolValue { Value = result.Result }
        };
    }

    public async Task<(bool Success, string? Result)> ReadFile(PullReviewCreatedEvent prEvent, string filePath) {
        if(prEvent is not BitbucketPullReviewCreatedEventDto request) {
           _logger.LogError($"{nameof(ReadFile)}: The type of event does not match the injected service in {nameof(ReadFile)}, returning early.");
           return (false, null);
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new(request); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(ReadFile)}: Provided event payload contained an invalid value, returning early.");
           return (false, null);
        }
 
        // this needs to always use the branch SHA to avoid routing issues.
        try {
            using(HttpRequestMessage message = new(HttpMethod.Get, $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/{filePath}")) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent));
                
                using HttpResponseMessage response = await client.SendAsync(message);
                // TODO: this is the entire response, need to get the actual response out of it.
                string content = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode) {
                    string[] results = new string[] { content };
                    if(string.IsNullOrWhiteSpace(results[0]))
                        throw new InvalidOperationException($"{nameof(ReadFile)}: Failed to fill array with response content, entry was invalid.");

                    return (false, null);
                } else {
                    _logger.LogError($"{nameof(ReadFile)}: Response was unsuccessful.");
                    return (false, null);
                }
            }
        } catch (Exception error) {
            _logger.LogError($"{nameof(ReadFile)}: Failed to fetch file contents. Error encountered: {error}.");
            return (false, null);
        } 
    }

}

