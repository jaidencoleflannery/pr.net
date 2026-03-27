using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringMessageDto {

    [JsonPropertyName("role")] 
    public string Role { get; set; } = "user";
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonIgnore]
    public string Path { get; set; } = string.Empty;

}