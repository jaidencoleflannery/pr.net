using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeRepositoryDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("links")]
    public ClaudeLinksDto Links { get; set; } = new ClaudeLinksDto();

}