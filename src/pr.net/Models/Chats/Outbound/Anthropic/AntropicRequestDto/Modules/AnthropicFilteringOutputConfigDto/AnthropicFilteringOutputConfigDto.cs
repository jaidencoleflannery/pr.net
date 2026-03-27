using System.Text.Json.Serialization;
using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringOutputConfigDto : OutputConfig {

    [JsonPropertyName("format")]
    public AnthropicFilteringFormatDto Format { get; set; } = new AnthropicFilteringFormatDto();

}