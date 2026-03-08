using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicProperties {

    [JsonPropertyName("content")]
    public AnthropicContent? Content { get; set; } = new AnthropicContent();

    [JsonPropertyName("inline")]
    public AnthropicInline? Inline { get; set; } = new AnthropicInline();

    // if parent.id is provided, the comment is a reply
    [JsonPropertyName("parent")]
    public AnthropicParent? Parent { get; set; } = new AnthropicParent();

    // makes comments invisible until approved
    [JsonPropertyName("pending")]
    public bool Pending { get; set; } = false;
    
}
