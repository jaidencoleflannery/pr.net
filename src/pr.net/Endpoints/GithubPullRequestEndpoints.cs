using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Github;

using static Microsoft.AspNetCore.Http.Results;

namespace pr.net.Endpoints;

public static class GithubPullRequestEndpoints {

    public static void MapGithubPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/github/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromServices] IValidator validator,
            [FromBody] GithubPullReviewCreatedEventDto prEvent
        ) => {
            // augment this line if you'd like to add functionality for other events.
            if(!validator.ValidateType(prEvent.Action.ToString()))
                return BadRequest(new { message = "Invalid event type." });

            // webhook secret validation.
            if(!await validator.ValidateWebhookSecretAsync(JsonSerializer.Serialize(prEvent)))
                return BadRequest(new { message = "Invalid webhook identifier." });

            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent);
            return Ok("Successfully posted reviews");
        });
    } 

}