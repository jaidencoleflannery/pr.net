using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringConfigDto {

    [JsonPropertyName("format")]
    public AnthropicFormatDto? Format { get; set; } = new AnthropicFormatDto();

}