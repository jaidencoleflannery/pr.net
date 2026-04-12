using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Configurations.Repo;

public class RepoConfiguration {

    public RepoProvider? Provider { get; set; } = null;

    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? ProviderString { get => field; init { Provider = ValidateRepoProvider(value); field = value; } }

    [Required]
    [ConfigurationKeyName("AcceptedEvents")]
    public List<string> AcceptedEvents { get; init; } = [];

    public IRepoProviderConfiguration? ActiveConfiguration => Provider switch {
      RepoProvider.Github => Github ?? throw new InvalidOperationException("Github configuration could not be found, please confirm values are properly set."),
      RepoProvider.Bitbucket => Bitbucket ?? throw new InvalidOperationException("Bitbucket configuration could not be found, please confirm values are properly set."),
      _ => throw new InvalidOperationException($"No Repository Provider was set, issue encountered in {nameof(RepoConfiguration)}")
    };

    [ConfigurationKeyName("Bitbucket")]
    public RepoBitbucketConfiguration? Bitbucket { get => field; init; }

    [ConfigurationKeyName("Github")]
    public RepoGithubConfiguration? Github { get => field; init; }

}