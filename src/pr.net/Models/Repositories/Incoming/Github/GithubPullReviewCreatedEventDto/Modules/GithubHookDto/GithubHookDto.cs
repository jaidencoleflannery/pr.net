using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubHookDto {

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("events")]
    public List<string> Events { get; set; } = new List<string>();

    [JsonPropertyName("config")]
    public GithubConfigDto Config { get; set; } = new GithubConfigDto();

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("test_url")]
    public string TestUrl { get; set; } = string.Empty;

    [JsonPropertyName("ping_url")]
    public string PingUrl { get; set; } = string.Empty;

    [JsonPropertyName("deliveries_url")]
    public string DeliveriesUrl { get; set; } = string.Empty;

    [JsonPropertyName("last_response")]
    public GithubLastResponseDto LastResponse { get; set; } = new GithubLastResponseDto();

}