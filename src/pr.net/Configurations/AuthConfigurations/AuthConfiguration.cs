using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Auth;

public class AuthConfiguration {
 
    [Required]
    [ConfigurationKeyName("Auth")] 
    public AuthProviderConfiguration? Auth { get => field; init; }
    
}