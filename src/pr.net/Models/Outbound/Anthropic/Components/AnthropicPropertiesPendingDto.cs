using System.Text.Json.Serialization;

public class AnthropicPropertiesPendingDto {

    [JsonPropertyName("type")]
    public string? Type { get; set; } = "boolean";
    
}