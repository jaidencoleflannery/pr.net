using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.HostProviders;

namespace pr.net.Configurations.Host;

public class HostConfiguration {

    public HostProvider Provider { get; set; }
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? ProviderString { get => field; init { Provider = ValidateHostProvider(value); field = value; } }

    public IHostProviderConfiguration? ActiveConfiguration => Provider switch {
      HostProvider.Amazon => Amazon ?? throw new InvalidOperationException("Amazon configuration could not be found, please confirm values are properly set."),
      HostProvider.Environment => null,
      _ => throw new InvalidOperationException($"No Host Provider was set, issue encountered in {nameof(HostProvider)}")
    };

    [ConfigurationKeyName("Amazon")]
    public HostAmazonConfiguration? Amazon { get => field; init; }

}