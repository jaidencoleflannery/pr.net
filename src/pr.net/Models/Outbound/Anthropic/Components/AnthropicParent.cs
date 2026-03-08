using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicParent {
    
    [JsonPropertyName("id")]
    public int? Id { get; set; } = null;

}