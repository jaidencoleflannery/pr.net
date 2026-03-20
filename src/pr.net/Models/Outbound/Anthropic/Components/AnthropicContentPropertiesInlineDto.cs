using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesInlineDto {
    
    [JsonPropertyName("to")]
    public AnthropicContentPropertiesRawDto To { get; set; } = new AnthropicContentPropertiesRawDto();

    [JsonPropertyName("path")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Path { get; set; } = null;

}