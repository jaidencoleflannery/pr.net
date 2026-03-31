using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubRepositoriesDto {

    [JsonPropertyName("id")]
    public int Issues { get; set; } = -1;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public GithubOwnerDto Owner { get; set; } = new GithubOwnerDto();

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("assignees_url")]
    public string AssigneesUrl { get; set; } = string.Empty;

}