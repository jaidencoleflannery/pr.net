using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoUsersConfiguration { 

    [Required]
    [ConfigurationKeyName("Filtering")]
    public bool Filtering { get; init; }

    [Required]
    [ConfigurationKeyName("AuthorizedUsers")]
    public required List<string> AuthorizedUsers { get; init; }

    [Required]
    [ConfigurationKeyName("Patterns")]
    public bool Patterns { get; init; }

    [Required]
    [ConfigurationKeyName("LimitRequests")]
    public RepoLimitRequestsConfiguration LimitRequests { get; set; } = new();

}