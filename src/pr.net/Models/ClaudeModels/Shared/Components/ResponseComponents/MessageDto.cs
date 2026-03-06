using System.Text.Json.Serialization;

namespace pr.net.Models;

public class MessageDto {
    [JsonPropertyName("user")] 
    public string Content { get; set; }= string.Empty;
}