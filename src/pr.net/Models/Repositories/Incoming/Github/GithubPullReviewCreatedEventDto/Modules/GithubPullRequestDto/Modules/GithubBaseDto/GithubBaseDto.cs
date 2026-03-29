using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubBaseDto { 

    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;
     
}