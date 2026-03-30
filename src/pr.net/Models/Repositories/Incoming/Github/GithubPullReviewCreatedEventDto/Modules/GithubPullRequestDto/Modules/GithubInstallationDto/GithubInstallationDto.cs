using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubInstallationDto { 

    [JsonPropertyName("id")]
    public long Id { get; set; } = -1;
     
}