using System.Text.Json.Serialization;

namespace pr.net.Models;

public class AnthropicPropertiesDto : PropertiesDto {

    [JsonPropertyName("content")]
    public AnthropicContentDto? Content { get; set; } = new AnthropicContentDto();

    [JsonPropertyName("inline")]
    public AnthropicInlineDto? Inline { get; set; } = new AnthropicInlineDto();

    // if parent.id is provided, the comment is a reply
    [JsonPropertyName("parent")]
    public AnthropicParentDto? Parent { get; set; } = new AnthropicParentDto();

    // makes comments invisible until approved
    [JsonPropertyName("pending")]
    public bool Pending { get; set; } = false;
    
}
