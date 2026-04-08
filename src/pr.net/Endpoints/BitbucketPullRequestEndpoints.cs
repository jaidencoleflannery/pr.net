using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;

using pr.net.Models.Bitbucket;

namespace pr.net.Endpoints;

public static class BitbucketPullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent,
            HttpRequest request
        ) => {
            // augment this line if you'd like to add functionality for other events.
            if(request.Headers["X-Event-Key"].ToString() is not "pullrequest:created")
                return;

            await orchestrator.ProcessNewPullRequest(prEvent);
        });
    } 

}