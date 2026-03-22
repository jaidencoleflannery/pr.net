using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Bitbucket;

public class BitbucketLinkDto {

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
    
}