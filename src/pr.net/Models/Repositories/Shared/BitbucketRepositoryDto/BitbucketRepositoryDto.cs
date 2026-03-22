using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketRepositoryDto {

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("links")]
    public BitbucketLinksDto Links { get; set; } = new BitbucketLinksDto();

}

