using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming.Anthropic;

public class AnthropicCacheCreation {

    [JsonPropertyName("ephemeral_5m_input_tokens")]
    public int Ephemeral5mInputTokens { get; set; } = -1;

    [JsonPropertyName("ephemeral_1h_input_tokens")]
    public int Ephemeral1hInputTokens { get; set; } = -1;

}