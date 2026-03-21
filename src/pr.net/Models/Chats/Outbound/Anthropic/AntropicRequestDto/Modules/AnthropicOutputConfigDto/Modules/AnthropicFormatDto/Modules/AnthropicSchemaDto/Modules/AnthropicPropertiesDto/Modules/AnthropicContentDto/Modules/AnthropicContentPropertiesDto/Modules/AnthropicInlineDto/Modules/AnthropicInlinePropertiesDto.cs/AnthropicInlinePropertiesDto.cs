using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicContentPropertiesInlinePropertiesDto {

    [JsonPropertyName("to")]
    public AnthropicRawDto To { get; set; } = new AnthropicRawDto() { Description = "The specific line number the comment should be attached to." };

    [JsonPropertyName("path")]
    public string? Path = null;

}