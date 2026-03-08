using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicSchema {
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public AnthropicProperties? Properties { get; set; } = new AnthropicProperties();

    [JsonPropertyName("required")]
    public string[] Required = [];

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;
}