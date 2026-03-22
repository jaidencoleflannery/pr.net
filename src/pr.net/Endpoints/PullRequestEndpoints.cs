using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Bitbucket;
using pr.net.Services.Orchestration;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] Orchestrator orchestrator,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent
        ) => orchestrator.ProcessNewPullRequest(prEvent));
    } 

}