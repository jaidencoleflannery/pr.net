using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using pr.net.Services.Tokens;

using pr.net.Configurations.Chat;

using pr.net.Models.Tooling;
using pr.net.Models.Bitbucket;

using static pr.net.Models.Tooling.PresetToolResponses;

namespace pr.net.Services.Tooling;

public class BitbucketToolClient(
    HttpClient client,
    ITokenService _tokenService,
    IOptions<ChatConfiguration> _chatConfiguration, 
    ILogger<BitbucketToolClient> _logger
) : IToolClient {

    private readonly uint _maxFileTreeDepth = 25;
    private readonly uint _fileTreePageLength = 100;

    public async ValueTask<ToolResponse> FetchFileTree(ToolParameters parameters) {
        if(parameters.PrEvent is null or not BitbucketPullReviewCreatedEventDto) {
           _logger.LogError($"{nameof(FetchFileTree)}: The type of event does not match the injected service, or is invalid, short circuiting.");
           return ToolFail();
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new((BitbucketPullReviewCreatedEventDto)parameters.PrEvent); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(FetchFileTree)}: Provided event payload contained an invalid value, short circuiting.");
           return ToolFail();
        }

        // https://api.bitbucket.org/2.0/repositories/{workspace}/{repo_slug}/src/{branch_or_commit}/?max_depth=25&pagelen=100&fields=values.path,values.type,next
        string url = $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/src/{metadata.CommitHash}/"
            + $"?max_depth={_maxFileTreeDepth}&pagelen={_fileTreePageLength}&fields=values.path,values.type,next";

        try {
            string? token = await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, parameters.PrEvent);
            if(string.IsNullOrWhiteSpace(token)) {
                _logger.LogError($"{nameof(FetchFileTree)}: Failed to retrieve a valid token, short circuiting.");
                return ToolFail();
            }

            List<BitbucketFileTreeEntryDto>? entries = await FetchFileTreePage(url, token, []);
            if(entries is null)
                return ToolFail();

            return new ToolResponse {
                Success = true,
                Result = [BuildFolderTree(entries)]
            };
        } catch (Exception error) {
            _logger.LogError($"{nameof(FetchFileTree)}: Failed to fetch file tree. Error encountered: {error}.");
            return ToolFail();
        }
    }

    private async ValueTask<List<BitbucketFileTreeEntryDto>?> FetchFileTreePage(string url, string token, List<BitbucketFileTreeEntryDto> accumulated) {
        using HttpRequestMessage message = new(HttpMethod.Get, url);
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        using HttpResponseMessage response = await client.SendAsync(message);
        if(!response.IsSuccessStatusCode) {
            _logger.LogError($"{nameof(FetchFileTree)}: Fetch request was unsuccessful.");
            return null;
        }

        string content = await response.Content.ReadAsStringAsync();
        if(string.IsNullOrWhiteSpace(content))
            // this is within the try/catch, don't throw upchain in tools.
            throw new InvalidOperationException($"{nameof(FetchFileTree)}: Response content was invalid, failed to fetch.");

        BitbucketFileTreeResponseDto? page = JsonSerializer.Deserialize<BitbucketFileTreeResponseDto>(content)
            ?? throw new InvalidOperationException($"{nameof(FetchFileTree)}: Failed to deserialize file tree response.");

        accumulated.AddRange(page.Values);

        return string.IsNullOrWhiteSpace(page.Next)
            ? accumulated
            : await FetchFileTreePage(page.Next, token, accumulated);
    }

    // expands the flat path list into an indented tree of directories and files.
    private static string BuildFolderTree(List<BitbucketFileTreeEntryDto> entries) {
        SortedDictionary<string, object?> root = new(StringComparer.Ordinal);

        foreach(BitbucketFileTreeEntryDto entry in entries) {
            string[] segments = entry.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if(segments.Length == 0)
                continue;

            // everything except the last segment is always a directory; the last
            // segment is a directory for commit_directory entries and a file otherwise.
            bool lastIsFile = entry.Type != "commit_directory";

            SortedDictionary<string, object?> current = root;
            for(int i = 0; i < segments.Length; i++) {
                bool isFile = lastIsFile && i == segments.Length - 1;
                if(current.TryGetValue(segments[i], out object? child) && child is SortedDictionary<string, object?> existing) {
                    current = existing;
                } else if(isFile) {
                    current[segments[i]] = null; // leaf file node.
                } else {
                    SortedDictionary<string, object?> next = new(StringComparer.Ordinal);
                    current[segments[i]] = next;
                    current = next;
                }
            }
        }

        StringBuilder builder = new();
        AppendFolderTree(root, builder, 0);
        return builder.Length > 0 ? builder.ToString() : "(empty repository)";
    }

    private static void AppendFolderTree(SortedDictionary<string, object?> node, StringBuilder builder, int depth) {
        foreach((string name, object? child) in node) {
            builder.Append(' ', depth * 2).Append(name);
            if(child is SortedDictionary<string, object?> children) {
                builder.Append('/').Append('\n');
                AppendFolderTree(children, builder, depth + 1);
            } else {
                builder.Append('\n');
            }
        }
    }

    public async ValueTask<ToolResponse> FetchFile(ToolParameters parameters) {
        if(parameters.PrEvent is null or not BitbucketPullReviewCreatedEventDto) {
           _logger.LogError($"{nameof(FetchFile)}: The type of event does not match the injected service, or is invalid, short circuiting.");
           return ToolFail();
        }

        if(parameters.ToolInput.Count() != 1) {
            _logger.LogError($"{nameof(FetchFile)}: Input for Fetch was invalid.");
           return ToolFail();
        }

        BitbucketPullReviewCreatedMetadataDto metadata = new((BitbucketPullReviewCreatedEventDto)parameters.PrEvent); // grab minimum metadata.
        if(string.IsNullOrWhiteSpace(metadata.CommitHash)
        || string.IsNullOrWhiteSpace(metadata.RepoSlug)) {
            _logger.LogError($"{nameof(FetchFile)}: Provided event payload contained an invalid value, short circuiting.");
           return ToolFail();
        }

        string? path = parameters.ToolInput.First();
        if(string.IsNullOrWhiteSpace(path)) {
            _logger.LogError($"{nameof(FetchFile)}: Path was invalid.");
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

