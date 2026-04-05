using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoBitbucketConfiguration : IRepoProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("RepoEmail")] 
    public string? RepoEmail { get => field; init; }

    [Required]
    [ConfigurationKeyName("Workspace")]
    public string? Workspace { get => field; init; }

    [Required]
    [ConfigurationKeyName("RepoSlug")]
    public string? RepoSlug { get => field; init; }

    [Required]
    [ConfigurationKeyName("PostCommentsUri")]
    public string? PostCommentsUri { get => field; init; }

}