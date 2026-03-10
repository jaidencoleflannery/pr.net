using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicRequestDto : RequestDto {

    public AnthropicRequestDto() { }

    [JsonPropertyName("messages")]
    public new List<AnthropicMessageDto> Messages { get; set; } = new List<AnthropicMessageDto>();

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;            

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } 

    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; set; } = null;

    /*
    [JsonPropertyName("output_config")]
    public AnthropicOutputConfig? OutputConfig { get; set; } = new AnthropicOutputConfig();
    */
 
}