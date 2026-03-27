using System.Text.Json.Serialization;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicFilteringFormatDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "json_schema";

    [JsonPropertyName("schema")]
    public AnthropicFilteringSchemaDto? Schema { get; set; } = new AnthropicFilteringSchemaDto();

}