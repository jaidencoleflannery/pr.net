using System.Text.Json.Serialization;

namespace pr.net.Models;

public class ClaudeRequestDto {

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;            

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("messages")]
    public List<MessageDto> Messages = new List<MessageDto>(); 

    [JsonPropertyName("system")]
    public string System { get; set; } = string.Empty;

}