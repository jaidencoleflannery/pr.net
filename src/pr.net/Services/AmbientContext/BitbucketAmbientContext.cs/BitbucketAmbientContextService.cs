using pr.net.Models.Bitbucket;

namespace pr.net.Services.Context;

public class BitbucketAmbientContextService : AmbientContextService<BitbucketPullReviewCreatedEventDto> {
    public BitbucketAmbientContextService(BitbucketPullReviewCreatedEventDto _event) {
        this.CreatedEvents.TryAdd(_event.PullRequest.Id, _event);
    }
}