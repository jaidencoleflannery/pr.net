using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoLimitRequestsConfiguration { 

    [Required]
    [ConfigurationKeyName("Limit")]
    public bool Limit { get; init; }

    [Required]
    [ConfigurationKeyName("MaxRequests")]
    public int MaxRequests { get; init; }
    
}