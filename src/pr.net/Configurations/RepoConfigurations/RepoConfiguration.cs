using System.ComponentModel.DataAnnotations;

using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Configurations.Repo;

public class RepoConfiguration {

    private RepoProvider _provider = RepoProvider.None;
 
    [Required]
    [ConfigurationKeyName("Provider")] 
    public string? Provider { get => field; init { _provider = ValidateRepoProvider(value); field = value; } }

    public IRepoProviderConfiguration? ActiveConfiguration => _provider switch {
      RepoProvider.Github => Github ?? throw new InvalidOperationException("Github configuration could not be found, please confirm values are properly set."),
      RepoProvider.Bitbucket => Bitbucket ?? throw new InvalidOperationException("Bitbucket configuration could not be found, please confirm values are properly set."),
      _ => throw new InvalidOperationException($"No Repository Provider was set, issue encountered in {nameof(RepoConfiguration)}")
    };

    [ConfigurationKeyName("Bitbucket")]
    public RepoBitbucketConfiguration? Bitbucket { get => field; init; }

    [ConfigurationKeyName("Github")]
    public RepoGithubConfiguration? Github { get => field; init; }

}