using Microsoft.AspNetCore.Mvc;
using pr.net.Services.Orchestration;
using pr.net.Models.Incoming.Generic;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] Orchestrator orchestrator,
            [FromBody] PullReviewCreatedEvent prEvent
        ) => orchestrator.ProcessNewPullRequest(prEvent));
    } 

}