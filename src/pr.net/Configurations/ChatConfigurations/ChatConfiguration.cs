using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Configurations.Chat;

public class ChatConfiguration {
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? Provider { get => field; init { ValidateChatProvider(value); field = value; } }

    [Required]
    [ConfigurationKeyName("Model")]
    public string? Model { get => field; init; }

    [Required]
    [ConfigurationKeyName("MaxTokens")]
    public long? MaxTokens { get => field; init; }

    [Required]
    [ConfigurationKeyName("Url")]
    public string? Url { get => field; init; }

    [Required]
    [ConfigurationKeyName("Timeout")]
    public int? Timeout { get => field; init; }

    [Required]
    [ConfigurationKeyName("Instructions")]
    public ChatInstructionsConfiguration? Instructions { get => field; init; }

    [Required]
    [ConfigurationKeyName("Filtering")]
    public ChatFilteringConfiguration? Filtering { get => field; init; }

}