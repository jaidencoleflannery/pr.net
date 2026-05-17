using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Chat;

public class ChatFilteringConfiguration {

    [Required]
    [ConfigurationKeyName("Filter")]
    public bool? Filter { get; init; }

    [Required]
    [ConfigurationKeyName("Timeout")]
    public TimeSpan? Timeout { get; init; }

    [Required]
    [ConfigurationKeyName("UseEmbedding")]
    public bool? UseEmbedding { get; init; }

    [Required]
    [ConfigurationKeyName("Model")]
    public string? Model { get; init; }

    [Required]
    [ConfigurationKeyName("MaxTokens")]
    public int? MaxTokens { get; init; }

}
