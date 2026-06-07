using pr.net.Models.Incoming.Generic;
using pr.net.Models.Tooling;
using pr.net.Models.Bitbucket;

namespace pr.net.Tooling;

public class ReadFileTreeTool(ILogger _logger) : IReadFileTreeTool {

    private ToolResponse _fail = new() {
        Success = false,
        Value = null
    };

    public async ValueTask<ToolResponse> InvokeTool(ToolParameters parameters) {
        if(parameters is not ReadFileTreeParameters input) {
            _logger.LogError($"{nameof(ReadFileTree)}: Failed to invoke tool, parameters given were invalid");
            return _fail;
        }

        (bool Success, string? Result) result = await ReadFileTree(input.prEvent);
        if(!result.Success) {
            _logger.LogError($"{nameof(ReadFileTree): Invocation of tool failed.}");
            return _fail;
        }

        return new ToolResponse {
            Success = result.Success,
            (ToolValue)result.Result;
        }
    }

    public async Task<(bool Success, string? Result)> ReadFileTree(PullReviewCreatedEvent prEvent) {
        if(prEvent is not BitbucketPullReviewCreatedEventDto request) {
           _logger.LogError($"{nameof(ReadFileTree)}: The type of event does not match the injected service in {nameof(BitbucketApiClient)}, returning early.");
           return (false, null);
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new(request); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(ReadFileTree)}: Provided event payload contained an invalid value, returning early.");
           return (false, null);
        }
 
        // this needs to always use the branch SHA to avoid routing issues.
        try {
            using(HttpRequestMessage message = new(HttpMethod.Get, $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}")) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent));
                
                using HttpResponseMessage response = await client.SendAsync(message);
                // TODO: this is the entire response, need to get the actual response out of it.
                string content = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode) {
                    string[] results = new string[] { content };
                    if(string.IsNullOrWhiteSpace(results[0]))
                        throw new InvalidOperationException($"{nameof(ReadFileTree)}: Failed to fill array with response content, entry was invalid.");

                    return (false, null);
                } else {
                    _logger.LogError($"{nameof(ReadFileTree)}: Response was unsuccessful.");
                    return (false, null);
                }
            }
        } catch (Exception error) {
            _logger.LogError($"{nameof(ReadFileTree)}: Failed to fetch file tree for repository. Error encountered: {error}.");
            return (false, null);
        } 
    }

}

