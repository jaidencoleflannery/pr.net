using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubAccessTokenResponseDto {

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public string ExpiresAt { get; set; } = string.Empty;

    [JsonPropertyName("permissions")]
    public GithubPermissionsDto Permissions { get; set; } = new GithubPermissionsDto();

    [JsonPropertyName("repositories")]
    public List<GithubRepositoriesDto> Repositories { get; set; } = new List<GithubRepositoriesDto>();

}