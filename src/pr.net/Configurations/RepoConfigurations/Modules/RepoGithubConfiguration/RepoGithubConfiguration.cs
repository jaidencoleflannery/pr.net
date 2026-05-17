using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoGithubConfiguration : IRepoProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("AppName")] 
    public string? AppName { get; init; }

    [Required]
    [ConfigurationKeyName("AppId")]
    public string? AppId { get; init; }

}
