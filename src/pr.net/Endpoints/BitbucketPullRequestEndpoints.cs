using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Bitbucket;

using static Microsoft.AspNetCore.Http.Results;

namespace pr.net.Endpoints;

public static class BitbucketPullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromServices] IValidator validator,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent,
            HttpRequest request
        ) => {
            // augment this line if you'd like to add functionality for other events.
            if(!validator.ValidateType(request.Headers["X-Event-Key"].ToString()))
                return BadRequest(new { message = "Invalid event type." });

            // webhook uuid validation (stored under the generic webhook secret token).
            if(!await validator.ValidateWebhookSecretAsync(request.Headers["X-Hook-UUID"].ToString()))
                return BadRequest(new { message = "Invalid webhook identifier." });

            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent);
            return Ok("Successfully posted reviews");
        });
    } 

}