using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;

using pr.net.Models.Github;

namespace pr.net.Endpoints;

public static class GithubPullRequestEndpoints {

    public static void MapGithubPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/github/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromBody] GithubPullReviewCreatedEventDto prEvent
        ) => {
            // augment this line if you'd like to add functionality for other events.
            if(prEvent.Action is not ("opened" or "reopened"))
                return;

            await orchestrator.ProcessNewPullRequest(prEvent);
        });
    } 

}