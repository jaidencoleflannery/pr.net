using System.Text.Json;

namespace pr.net.Models.Outbound.Anthropic;

public static class AnthropicSchema {
    public static readonly Dictionary<string, JsonElement> FilterRequestSchema = new() {
        ["type"] = JsonSerializer.SerializeToElement("object"),
        ["properties"] = JsonSerializer.SerializeToElement(new {
            worthReview = new { type = "boolean" },
        }),
        ["required"] = JsonSerializer.SerializeToElement(new[] { "worthReview" }),
        ["additionalProperties"] = JsonSerializer.SerializeToElement(false)
    };
}