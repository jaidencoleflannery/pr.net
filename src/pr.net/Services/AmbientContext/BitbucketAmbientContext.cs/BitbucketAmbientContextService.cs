using pr.net.Models.Bitbucket;

namespace pr.net.Services.Context;

public class BitbucketAmbientContextService(BitbucketPullReviewCreatedEventDto _event) : AmbientContextService<BitbucketPullReviewCreatedEventDto>(_event) { }