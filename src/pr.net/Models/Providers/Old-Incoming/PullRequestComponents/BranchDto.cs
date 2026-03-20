using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming;

public class BranchDto {

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}