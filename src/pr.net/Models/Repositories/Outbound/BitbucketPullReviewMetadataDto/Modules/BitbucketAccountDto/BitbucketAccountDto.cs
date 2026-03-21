using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Bitbucket;

public class BitbucketAccountDto {
    
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("links")]
    public BitbucketLinksDto Links { get; set; } = new BitbucketLinksDto();

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("account_id")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

}