using System.Text.Json.Serialization;

namespace pr.net.Models;

public class LinkDto {

    [JsonPropertyName("href")]
    public string Hred { get; set; } = string.Empty;
    
}