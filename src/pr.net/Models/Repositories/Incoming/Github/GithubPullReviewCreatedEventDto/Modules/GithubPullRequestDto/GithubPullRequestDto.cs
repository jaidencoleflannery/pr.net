using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubPullRequestDto {

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty; 

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [JsonPropertyName("diff_url")]
    public string DiffUrl { get; set; } = string.Empty;

    [JsonPropertyName("locked")]
    public bool Locked { get; set; } = false;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public GithubUserDto User { get; set; } = new GithubUserDto();

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonPropertyName("closed_at")]
    public string ClosedAt { get; set; } = string.Empty;

    [JsonPropertyName("merged_at")]
    public string MergedAt { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("labels")]
    public List<GithubLabelsDto> Labels { get; set; } = [];

    [JsonPropertyName("requested_reviewers")]
    public GithubRequestedReviewersDto RequestedReviewers { get; set; } = new GithubRequestedReviewersDto();

    [JsonPropertyName("draft")]
    public bool Draft { get; set; } = false;

    [JsonPropertyName("commits_url")]
    public string CommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("review_comments_url")]
    public string ReviewCommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("head")]
    public GithubHeadDto Head { get; set; } = new GithubHeadDto();

    [JsonPropertyName("base")]
    public GithubBaseDto Base { get; set; } = new GithubBaseDto();

    [JsonPropertyName("merged")]
    public bool Merged { get; set; } = false;

    [JsonPropertyName("comments")]
    public string Comments { get; set; } = string.Empty;

    [JsonPropertyName("commits")]
    public int Commits { get; set; } = -1;

    [JsonPropertyName("additions")]
    public int Additions { get; set; } = -1;

    [JsonPropertyName("deletions")]
    public int Deletions { get; set; } = -1;

    [JsonPropertyName("changed_files")]
    public int ChangedFiles { get; set; } = -1;

}