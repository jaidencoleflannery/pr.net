using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubRepositoryDto {

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("branches_url")]
    public string BranchesUrl { get; set; } = string.Empty;

    [JsonPropertyName("blobs_url")]
    public string BlobsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_refs_url")]
    public string GitRefsUrl { get; set; } = string.Empty;

    [JsonPropertyName("trees_url")]
    public string TreesUrl { get; set; } = string.Empty;

    [JsonPropertyName("languages_url")]
    public string LanguagesUrl { get; set; } = string.Empty;

    [JsonPropertyName("commits_url")]
    public string CommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("git_commits_url")]
    public string GitCommitsUrl { get; set; } = string.Empty;

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("issue_comment_url")]
    public string IssueCommentUrl { get; set; } = string.Empty;

    [JsonPropertyName("contents_url")]
    public string ContentsUrl { get; set; } = string.Empty;

    [JsonPropertyName("compare_url")]
    public string CompareUrl { get; set; } = string.Empty;

    [JsonPropertyName("issues_url")]
    public string issuesUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("pulls_url")]
    public string PullsUrl { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("pushed_at")]
    public string PushedAt { get; set; } = string.Empty;

    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; } = string.Empty;

    [JsonPropertyName("ssh_url")]
    public string SshUrl { get; set; } = string.Empty;

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public int Size { get; set; } = -1;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("open_issues_count")]
    public int OpenIssuesCount { get; set; } = -1;

    [JsonPropertyName("has_pull_requests")]
    public bool HasPullRequests { get; set; } = true;

}