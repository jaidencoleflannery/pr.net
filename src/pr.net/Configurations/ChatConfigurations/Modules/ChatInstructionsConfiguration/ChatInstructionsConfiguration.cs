using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Chat;

public class ChatInstructionsConfiguration {

    [Required] 
    [ConfigurationKeyName("Provider")]
    public string? Provider { get => field; init; }

}