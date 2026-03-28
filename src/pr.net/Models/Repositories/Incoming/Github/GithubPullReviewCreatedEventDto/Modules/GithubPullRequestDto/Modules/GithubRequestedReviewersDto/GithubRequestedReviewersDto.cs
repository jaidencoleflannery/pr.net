using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubRequestedReviewersDto { 

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;
     
}