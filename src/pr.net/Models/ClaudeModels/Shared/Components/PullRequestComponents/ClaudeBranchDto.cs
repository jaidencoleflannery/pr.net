using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeBranchDto {

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}