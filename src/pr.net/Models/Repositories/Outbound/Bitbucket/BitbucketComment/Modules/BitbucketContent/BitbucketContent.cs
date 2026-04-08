using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketContent {

    [JsonPropertyName("raw")]
    public string Raw { get; set; } = string.Empty;

}