using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicContentPropertiesInlinePropertiesDto {

    [JsonPropertyName("to")]
    public AnthropicContentPropertiesRawDto To { get; set; } = new AnthropicContentPropertiesRawDto() { Description = "The specific line number the comment should be attached to." };

    [JsonPropertyName("path")]
    public string? Path = null;

}