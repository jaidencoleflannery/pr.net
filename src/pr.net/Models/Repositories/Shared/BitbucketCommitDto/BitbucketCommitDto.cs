using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketCommitDto {

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

}