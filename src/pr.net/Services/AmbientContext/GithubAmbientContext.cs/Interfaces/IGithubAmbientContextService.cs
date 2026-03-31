using pr.net.Models.Github;

namespace pr.net.Services.Context;

public interface IGithubAmbientContextService : IAmbientContextService<GithubPullReviewCreatedEventDto> { }