using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Bitbucket;

public class BitbucketCommitDto {

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

}