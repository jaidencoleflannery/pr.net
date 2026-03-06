using System.Text.Json.Serialization;

namespace pr.net.Models;

public class LinksDto {
    
    [JsonPropertyName("self")]
    public LinkDto Self { get; set; }

    [JsonPropertyName("avatar")]
    public LinkDto Avatar { get; set; }

    [JsonPropertyName("html")]
    public LinkDto Html { get; set; }

}