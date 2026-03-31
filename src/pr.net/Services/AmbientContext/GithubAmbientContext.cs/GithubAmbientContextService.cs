using pr.net.Models.Github;

namespace pr.net.Services.Context;

public class GithubAmbientContextService(GithubPullReviewCreatedEventDto _event) : AmbientContextService<GithubPullReviewCreatedEventDto>(_event) { }