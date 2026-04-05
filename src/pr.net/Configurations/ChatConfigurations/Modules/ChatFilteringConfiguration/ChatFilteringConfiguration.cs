using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Chat;

public class ChatFilteringConfiguration {

    [Required]
    [ConfigurationKeyName("Filter")]
    public bool? Filter { get => field; init; }

    [Required]
    [ConfigurationKeyName("Timeout")]
    public int? Timeout { get => field; init; }

    [Required]
    [ConfigurationKeyName("UseEmbedding")]
    public bool? UseEmbedding { get => field; init; }

    [Required]
    [ConfigurationKeyName("Model")]
    public string? Model { get => field; init; }

    [Required]
    [ConfigurationKeyName("MaxTokens")]
    public int? MaxTokens { get => field; init; }

}