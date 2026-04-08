using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketInline {

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public int To { get; set; } = -1;

}