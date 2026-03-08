using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicParentDto {
    
    [JsonPropertyName("id")]
    public int? Id { get; set; } = null;

}