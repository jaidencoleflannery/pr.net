using System.Text.Json.Serialization;
using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicRequestDto : Request {

    [JsonPropertyName("messages")]
    public List<AnthropicMessageDto> Messages { get; set; } = new List<AnthropicMessageDto>();

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } 

    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; set; } = null;
 
    [JsonPropertyName("output_config")]  
    public OutputConfig OutputConfig { get; set; } = new AnthropicOutputConfig();

}