using System.Text.Json.Serialization;

namespace pr.net.Models;

public class PostPullRequestDto {
    
    [JsonPropertyName("type")]
    public string? Type { get; set; } = "pullrequest_comment";

    [JsonPropertyName("content")]
    public PostPullRequestContentDto Content { get; set; } = new PostPullRequestContentDto();

    [JsonPropertyName("inline")]
    public PostPullRequestInlineDto Inline { get; set; } = new PostPullRequestInlineDto();
}