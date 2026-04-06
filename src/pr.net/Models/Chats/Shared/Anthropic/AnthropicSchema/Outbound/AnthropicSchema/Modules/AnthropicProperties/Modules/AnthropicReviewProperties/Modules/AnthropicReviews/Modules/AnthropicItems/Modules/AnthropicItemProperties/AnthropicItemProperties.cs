using System.Text.Json.Serialization;

namespace pr.net.Models.Anthropic;

public class AnthropicItemProperties : IAnthropicProperties {

    [JsonPropertyName("body")]
    public AnthropicStringType Body { get; set; } = new(); 

    [JsonPropertyName("line")]
    public AnthropicIntType Line { get; set; } = new();  

    public List<string> GetRequiredFields() => ["body", "line"];

}