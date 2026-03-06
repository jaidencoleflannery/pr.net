using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeLinksDto {
    
    [JsonPropertyName("self")]
    public ClaudeLinkDto Self { get; set; } = new ClaudeLinkDto();

    [JsonPropertyName("avatar")]
    public ClaudeLinkDto Avatar { get; set; } = new ClaudeLinkDto();

    [JsonPropertyName("html")]
    public ClaudeLinkDto Html { get; set; } = new ClaudeLinkDto();

    [JsonPropertyName("diff")]
    public ClaudeLinkDto? Diff { get; set; } = null;

}