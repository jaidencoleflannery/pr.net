using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming;

public class LinksDto {
    
    [JsonPropertyName("self")]
    public LinkDto Self { get; set; } = new LinkDto();

    [JsonPropertyName("avatar")]
    public LinkDto Avatar { get; set; } = new LinkDto();

    [JsonPropertyName("html")]
    public LinkDto Html { get; set; } = new LinkDto();

    [JsonPropertyName("diff")]
    public LinkDto? Diff { get; set; } = null;

}