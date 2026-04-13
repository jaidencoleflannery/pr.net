using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicPatternProperties : IAnthropicProperties {

    [JsonPropertyName("id")]
    public AnthropicIntType Id { get; set; } = new();


    [JsonPropertyName("pattern")]
    public AnthropicStringType Pattern { get; set; } = new();

    public List<string> GetRequiredFields() => ["pattern", "pattern"];

}