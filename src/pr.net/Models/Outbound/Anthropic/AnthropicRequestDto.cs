using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicRequestDto : RequestDto {

    public AnthropicRequestDto() { }

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;            

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("messages")]
    public List<MessageDto> Messages = new List<MessageDto>(); 

    [JsonPropertyName("system")]
    public string System { get; set; } = string.Empty;

    [JsonPropertyName("output_config")]
    public AnthropicOutputConfig? OutputConfig { get; set; } = new AnthropicOutputConfig();

}