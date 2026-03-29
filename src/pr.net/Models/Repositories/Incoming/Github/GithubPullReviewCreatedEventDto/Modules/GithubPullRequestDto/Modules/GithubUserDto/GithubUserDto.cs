using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubUserDto { 

    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

}