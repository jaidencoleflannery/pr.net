using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Context;

using pr.net.Models.Github;

namespace pr.net.Endpoints;

public static class GithubPullRequestEndpoints {

    public static void MapGithubPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/github/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] Orchestrator orchestrator,
            [FromServices] GithubAmbientContextService ambientContext,
            [FromBody] GithubPullReviewCreatedEventDto prEvent
            ) => {
                ambientContext.CreatedEvents.TryAdd(prEvent.PullRequest.Id, prEvent);
                return orchestrator.ProcessNewPullRequest();
            }
        );
    } 

}