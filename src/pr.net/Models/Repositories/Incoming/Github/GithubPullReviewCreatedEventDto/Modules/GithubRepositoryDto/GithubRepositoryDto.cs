using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubRepositoryDto { 

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("clone_url")]
    public string CloneUrl { get; set; } = string.Empty;

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public GithubOwnerDto Owner { get; set; } = new GithubOwnerDto();

}