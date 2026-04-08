using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.HostProviders;

namespace pr.net.Configurations.Chat;

public class HostConfiguration {

    public HostProvider Provider { get; set; }
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? ProviderString { get => field; init { Provider = ValidateHostProvider(value); field = value; } }

}