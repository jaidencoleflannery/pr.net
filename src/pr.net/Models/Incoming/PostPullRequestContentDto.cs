using System.Text.Json.Serialization;

namespace pr.net.Models;

public class PostPullRequestContentDto {
    
    [JsonPropertyName("raw")]
    public string? raw { get; set; } = null;

}