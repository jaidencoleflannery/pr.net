using System.Text.Json.Serialization;

namespace pr.net.Models.Bitbucket;

public class BitbucketFileTreeResponseDto {

    [JsonPropertyName("values")]
    public List<BitbucketFileTreeEntryDto> Values { get; set; } = new();

    [JsonPropertyName("next")]
    public string? Next { get; set; }

}
