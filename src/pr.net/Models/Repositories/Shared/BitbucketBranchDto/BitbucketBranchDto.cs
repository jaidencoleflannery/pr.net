using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Bitbucket;

public class BitbucketBranchDto {

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}