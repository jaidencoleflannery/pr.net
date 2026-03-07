using System.Text.Json.Serialization;

namespace pr.net.Models;

public class LinkDto {

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
    
}