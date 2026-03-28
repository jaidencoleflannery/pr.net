using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubConfigDto {

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("insecure_ssl")]
    public int InsecureSsl { get; set; } = -1;

    [JsonPropertyName("secret")]
    public string Secret { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

}