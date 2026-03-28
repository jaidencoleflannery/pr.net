using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubLastResponseDto {

    [JsonPropertyName("code")]
    public int Code { get; set; } = -1;

    [JsonPropertyName("status")]
    public string InsecureSsl { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

}