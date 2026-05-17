using System.ComponentModel.DataAnnotations;

namespace pr.net.Configurations.Repo;

public class RepoBitbucketConfiguration : IRepoProviderConfiguration {
 
    [Required]
    [ConfigurationKeyName("RepoEmail")] 
    public string? RepoEmail { get; init; }

    [Required]
    [ConfigurationKeyName("Workspace")]
    public string? Workspace { get; init; }

    [Required]
    [ConfigurationKeyName("RepoSlug")]
    public string? RepoSlug { get; init; }

    [Required]
    [ConfigurationKeyName("PostCommentsUri")]
    public string? PostCommentsUri { get; init; }

}
