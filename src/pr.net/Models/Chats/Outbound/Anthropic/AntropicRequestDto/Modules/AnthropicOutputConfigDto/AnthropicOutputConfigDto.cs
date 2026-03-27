using System.Text.Json.Serialization;
using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicOutputConfig : OutputConfig {

    [JsonPropertyName("format")]
    public AnthropicFormatDto? Format { get; set; } = new AnthropicFormatDto();

}