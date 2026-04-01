using System.Text.Json.Serialization;

using Anthropic.Core;
using Anthropic.Models.Messages;

using pr.net.Models.Outbound.Generic;

namespace pr.net.Models.Outbound.Anthropic;

public class AnthropicRequestDto : ChatRequest {

    [JsonPropertyName("messages")]
    public List<MessageParam>  Messages { get; set; } = new List<MessageParam>();

    [JsonPropertyName("model")]
    public ApiEnum<string, Model> Model { get; set; } = string.Empty;

    [JsonPropertyName("max_tokens")]
    public long MaxTokens { get; set; } 

    [JsonPropertyName("output_config")]  
    public AnthropicOutputConfig OutputConfig { get; set; } = new AnthropicOutputConfig();

}