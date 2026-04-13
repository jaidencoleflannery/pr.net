using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoPatternsConfiguration { 

    [Required]
    [ConfigurationKeyName("Limit")]
    public bool Filtering { get; init; }

    [Required]
    [ConfigurationKeyName("PatternHistoryLength")]
    public int PatternHistoryLength { get; init; }
    
}