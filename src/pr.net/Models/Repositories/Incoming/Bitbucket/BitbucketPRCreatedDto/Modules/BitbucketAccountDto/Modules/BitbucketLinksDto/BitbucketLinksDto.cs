using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Bitbucket;

public class BitbucketLinksDto {
    
    [JsonPropertyName("self")]
    public BitbucketLinkDto Self { get; set; } = new BitbucketLinkDto();

    [JsonPropertyName("avatar")]
    public BitbucketLinkDto Avatar { get; set; } = new BitbucketLinkDto();

    [JsonPropertyName("html")]
    public BitbucketLinkDto Html { get; set; } = new BitbucketLinkDto();

    [JsonPropertyName("diff")]
    public BitbucketLinkDto Diff { get; set; } = new BitbucketLinkDto();

}