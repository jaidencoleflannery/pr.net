using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicMessageDto : MessageDto {

    [JsonPropertyName("role")] 
    public string Role { get; set; } = "user";
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

}