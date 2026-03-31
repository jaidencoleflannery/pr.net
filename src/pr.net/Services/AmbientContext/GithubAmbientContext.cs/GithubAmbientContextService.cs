using pr.net.Models.Github;

namespace pr.net.Services.Context;

public class GithubAmbientContextService : AmbientContextService<GithubPullReviewCreatedEventDto> {
    public GithubAmbientContextService(GithubPullReviewCreatedEventDto _event) {
        this.CreatedEvents.TryAdd(_event.PullRequest.Id, _event);
    }
}