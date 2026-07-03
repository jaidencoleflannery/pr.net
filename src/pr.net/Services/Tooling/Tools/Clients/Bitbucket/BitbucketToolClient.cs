using System.Text.Json;

using Microsoft.Extensions.Options;

using pr.net.Services.Tokens;
using pr.net.Services.Tooling;

using pr.net.Configurations.Chat;

using pr.net.Models.Tooling;
using pr.net.Models.Bitbucket;
using pr.net.Models.Tooling.FetchFileTree;

namespace pr.net.Services.Tooling;

public class BitbucketToolClient(
    HttpClient client,
    ITokenService _tokenService,
    ILogger<BitbucketToolClient> _logger
) : IToolClient { 

    private readonly uint _maxFileTreeDepth = 25; 
    private readonly uint _fileTreePageLength = 100;
    private readonly uint _maximumFileSize = 50_000;

    public async ValueTask<ToolResponse> FetchFileTree(ToolParameters parameters, ToolMetadata toolMetadata) {
        ToolResponse toolFail = new() {
            Success = false,
            ToolName = toolMetadata.Name,
            Description = toolMetadata.Description
        };

        if(parameters is null) {
            _logger.LogError($"{nameof(FetchFileTree)}: Provided parameters were null, tool invocation failed.");
           return toolFail;
        }

        if(parameters.PrEvent is null or not BitbucketPullReviewCreatedEventDto) {
           _logger.LogError($"{nameof(FetchFileTree)}: The type of event does not match the injected service, or is invalid, short circuiting.");
           return toolFail;
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new((BitbucketPullReviewCreatedEventDto)parameters.PrEvent); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(FetchFileTree)}: Provided event payload contained an invalid value, short circuiting.");
           return toolFail;
        }

        // https://api.bitbucket.org/2.0/repositories/{workspace}/{repo_slug}/src/{branch_or_commit}/?max_depth=25&pagelen=100&fields=values.path,values.type,next
        string url = $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/"
            + $"?max_depth={_maxFileTreeDepth}&pagelen={_fileTreePageLength}&fields=values.path,values.type,next";

        try {
            string? token = await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, parameters.PrEvent);
            if(string.IsNullOrWhiteSpace(token)) {
                _logger.LogError($"{nameof(FetchFileTree)}: Failed to retrieve a valid token, short circuiting.");
                return toolFail;
            }

            uint recursionDepth = 0;
            List<BitbucketFileTreeEntryDto>? entries = await FetchFileTreePage(url, token, recursionDepth, []);
            if(entries is null)
                return toolFail;

            return new ToolResponse {
                Success = true,
                ToolName = toolMetadata.Name,
                Description = toolMetadata.Description,
                Result = [BuildFolderTree(entries)]
            }; 
        } catch (Exception error) {
            _logger.LogError($"{nameof(FetchFileTree)}: Failed to fetch file tree. Error encountered: {error}.");
            return toolFail;
        }
    }

    private async ValueTask<List<BitbucketFileTreeEntryDto>?> FetchFileTreePage(string url, string token, uint recursionDepth, List<BitbucketFileTreeEntryDto> accumulated) {
        if(recursionDepth > _maxFileTreeDepth) {
            _logger.LogError($"{nameof(FetchFileTreePage)}: Failed to fetch file tree page, max depth reached.");
            return null;
        }

        using HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        using HttpResponseMessage response = await client.SendAsync(message);
        if(!response.IsSuccessStatusCode) {
            _logger.LogError($"{nameof(FetchFileTreePage)}: Fetch request was unsuccessful.");
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();
        if(string.IsNullOrWhiteSpace(content)) {
            _logger.LogError($"{nameof(FetchFileTreePage)}: Response content was invalid, failed to fetch.");
            return null;
        }

        BitbucketFileTreeResponseDto? page = JsonSerializer.Deserialize<BitbucketFileTreeResponseDto>(content);
        if(page == null) {
            _logger.LogError($"{nameof(FetchFileTreePage)}: Failed to deserialize file tree response.");
            return null;
        }

        accumulated.AddRange(page.Values);

        if(string.IsNullOrWhiteSpace(page.Next)) {
            return accumulated;
        } else {
            List<BitbucketFileTreeEntryDto>? treePage = await FetchFileTreePage(page.Next, token, ++recursionDepth, accumulated);
            if(treePage == null) {
                accumulated.Add(new() {
                    Path = "Failed to fetch.",
                    Type = "commit_directory"
                });

                return accumulated;
            }

            return treePage;
        }
    }

    // convert repo structure string into json structure of folders + files.
    private static string BuildFolderTree(List<BitbucketFileTreeEntryDto> entries) {
        List<FetchFileTreeFile> root = [];

        foreach(BitbucketFileTreeEntryDto entry in entries) {
            string[] segments = entry.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if(segments.Length == 0)
                continue;

            bool isFile = (entry.Type != "commit_directory");

            List<FetchFileTreeFile> current = root;
            for(int cursor = 0; cursor < segments.Length; cursor++) {
                bool leafIsFile = ((cursor == segments.Length - 1) && isFile);

                FetchFileTreeFile? node = current.FirstOrDefault(child => child.Name == segments[cursor]);
                if(node is null) {
                    node = new FetchFileTreeFile {
                        Name = segments[cursor],
                        Type = leafIsFile ? "file" : "directory"
                    };
                    current.Add(node);
                }

                if(leafIsFile)
                    break;

                current = node.Children;
            }
        }

        return JsonSerializer.Serialize(root);
    }

    public async ValueTask<ToolResponse> FetchFile(ToolParameters parameters, ToolMetadata toolMetadata) {
        ToolResponse toolFail = new() {
            Success = false,
            ToolName = toolMetadata.Name,
            Description = toolMetadata.Description,
            Result = []
        };

        if(parameters is null) {
            _logger.LogError($"{nameof(FetchFile)}: Provided parameters were null, short circuiting.");
           return toolFail;
        }

        if(parameters.PrEvent is null or not BitbucketPullReviewCreatedEventDto) {
           _logger.LogError($"{nameof(FetchFile)}: The type of event does not match the injected service, or is invalid, short circuiting.");
           return toolFail;
        }

        if(parameters.ToolInput is null || parameters.ToolInput.Count() != 1) {
            _logger.LogError($"{nameof(FetchFile)}: Input for Fetch was invalid.");
           return toolFail;
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new((BitbucketPullReviewCreatedEventDto)parameters.PrEvent); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(FetchFile)}: Provided event payload contained an invalid value, short circuiting.");
           return toolFail;
        }

        string? path = parameters.ToolInput.First();
        if(string.IsNullOrWhiteSpace(path)) {
            _logger.LogError($"{nameof(FetchFile)}: Path was invalid.");
           return toolFail;
        }

        // this needs to always use the branch sha to avoid routing issues.
        try {
            using(HttpRequestMessage message = new(
                HttpMethod.Get, 
                $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/{path}")
            ) {

                string? token = await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, parameters.PrEvent);
                if(string.IsNullOrWhiteSpace(token)) {
                    _logger.LogError($"{nameof(FetchFile)}: Failed to retrieve token.");
                    return toolFail;
                }

                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                using HttpResponseMessage response = await client.SendAsync(message);
                string content = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode) {
                    if(string.IsNullOrWhiteSpace(content)) {
                        _logger.LogError($"{nameof(FetchFile)}: Response content was invalid, failed to fetch.");
                        return toolFail;
                    }

                    return new ToolResponse {
                        Success = true,
                        ToolName = toolMetadata.Name,
                        Description = toolMetadata.Description,
                        Result = (content.Length > _maximumFileSize)
                            ? [content]
                            : ["File size was too large to read."]
                    };
                }
                _logger.LogError($"{nameof(FetchFile)}: Fetch request was unsuccessful.");
                return toolFail;
            }
        } catch (Exception error) {
            _logger.LogError($"{nameof(FetchFile)}: Failed to fetch file contents. Error encountered: {error}.");
            return toolFail;
        }
    }

}
