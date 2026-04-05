using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.AuthProviders;

namespace pr.net.Configurations.Auth;

public class AuthProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? Provider { get => field; init { ValidateAuthProvider(value); field = value; } }
    
}