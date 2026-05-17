using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Host;

public class HostAmazonConfiguration : IHostProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("SecretsManagerPath")] 
    public string? SecretsManagerPath { get; init; }

}
