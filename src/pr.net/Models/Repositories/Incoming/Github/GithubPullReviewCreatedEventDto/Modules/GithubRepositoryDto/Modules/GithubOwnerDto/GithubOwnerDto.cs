using System.Text.Json.Serialization;

namespace pr.net.Models.Github;

public class GithubOwnerDto { 

    [JsonPropertyName("login")] 
    public string Login { get; set; } = string.Empty;

}