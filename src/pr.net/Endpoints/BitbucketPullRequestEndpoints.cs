using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Context;

using pr.net.Models.Bitbucket;

namespace pr.net.Endpoints;

public static class BitbucketPullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] Orchestrator orchestrator,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent
        ) => {
            return orchestrator.ProcessNewPullRequest(prEvent);
        });
    } 

}