using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Configurations.Chat;

public class ChatConfiguration {

    public ChatProvider? Provider { get; set; } = null;
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? ProviderString { get; init { Provider = ValidateChatProvider(value); field = value; } }

    [Required]
    [ConfigurationKeyName("Model")]
    public string? Model { get; init; }

    [Required]
    [ConfigurationKeyName("MaxTokens")]
    public long? MaxTokens { get; init; }

    [Required]
    [ConfigurationKeyName("Url")]
    public string? Url { get; init; }

    [Required]
    [ConfigurationKeyName("Timeout")]
    public TimeSpan? Timeout { get; init; }

    [Required]
    [ConfigurationKeyName("Instructions")]
    public ChatInstructionsConfiguration? Instructions { get; init; }

    [Required]
    [ConfigurationKeyName("Filtering")]
    public ChatFilteringConfiguration? Filtering { get; init; }

}
