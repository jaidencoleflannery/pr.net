using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubLabelsDto { 

    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
     
}