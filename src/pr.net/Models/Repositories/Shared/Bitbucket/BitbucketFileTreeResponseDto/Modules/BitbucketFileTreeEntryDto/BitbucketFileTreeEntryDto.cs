using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketFileTreeEntryDto {

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

}
