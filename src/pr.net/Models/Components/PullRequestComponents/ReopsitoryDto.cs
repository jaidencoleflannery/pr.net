using System.Text.Json.Serialization;

namespace pr.net.Models;

public class RepositoryDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("links")]
    public LinksDto Links { get; set; } = new LinksDto();

}