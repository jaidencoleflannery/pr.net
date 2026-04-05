using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoGithubConfiguration : IRepoProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("AppName")] 
    public string? AppName { get => field; init; }

    [Required]
    [ConfigurationKeyName("AppId")]
    public string? AppId { get => field; init; }

}