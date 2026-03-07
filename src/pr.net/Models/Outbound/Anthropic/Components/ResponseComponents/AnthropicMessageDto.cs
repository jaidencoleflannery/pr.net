using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicMessageDto : MessageDto {

    [JsonPropertyName("role")] 
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content = string.Empty;

}