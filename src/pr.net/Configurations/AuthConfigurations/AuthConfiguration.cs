using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Auth;

public class AuthConfiguration {
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? Provider { get; init; }
    
}
