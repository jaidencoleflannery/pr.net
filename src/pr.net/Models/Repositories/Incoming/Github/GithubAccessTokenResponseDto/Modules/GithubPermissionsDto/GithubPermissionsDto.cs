using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubPermissionsDto {

    [JsonPropertyName("issues")]
    public string Issues { get; set; } = string.Empty;

    [JsonPropertyName("contents")]
    public string Contents { get; set; } = string.Empty;

}