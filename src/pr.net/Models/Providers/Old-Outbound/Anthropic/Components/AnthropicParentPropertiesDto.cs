using System.Text.Json.Serialization;

public class AnthropicParentPropertiesDto {

    [JsonPropertyName("id")]
    public AnthropicParentPropertiesIdDto Id = new AnthropicParentPropertiesIdDto();
}