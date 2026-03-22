using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketLinkDto {

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
    
}