using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketComment {

    [JsonPropertyName("content")]
    public BitbucketContent Content { get; set; } = new();

    [JsonPropertyName("inline")] 
    public BitbucketInline Inline { get; set; } = new();

}