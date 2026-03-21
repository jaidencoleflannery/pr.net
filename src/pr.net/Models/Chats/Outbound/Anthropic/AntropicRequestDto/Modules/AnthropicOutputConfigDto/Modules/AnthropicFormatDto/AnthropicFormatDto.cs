using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFormatDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "json_schema";

    [JsonPropertyName("schema")]
    public AnthropicSchemaDto? Schema { get; set; } = new AnthropicSchemaDto();

}