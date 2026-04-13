using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Github;

using static pr.net.Models.Enums.Events;
using static Microsoft.AspNetCore.Http.Results;
using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Endpoints;

public static class GithubPullRequestEndpoints {

    public static void MapGithubPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/github/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromServices] IWebhookValidator validator,
            HttpRequest request
        ) => {
            // read body directly as a string so we can encode it and compare to the provided webhook secret.
            using var reader = new StreamReader(request.Body);
            string body = await reader.ReadToEndAsync();

            // validate webhook secret.
            string secretHeader = request.Headers["X-Hub-Signature-256"].ToString();
            if(string.IsNullOrWhiteSpace(secretHeader) || !await validator.ValidateWebhookSecretAsync(secretHeader, body))
                return BadRequest(new { message = "Unauthorized." });

            // deserialize the string so we can pull values.
            GithubPullReviewCreatedEventDto prEvent = JsonSerializer.Deserialize<GithubPullReviewCreatedEventDto>(body)
                ?? throw new InvalidOperationException("Unexpected error encountered attempting to deserialize request payload."); 

            // validate webhook event type.
            if(prEvent.Action is null || !ValidateEvent(prEvent.Action, RepoProvider.Github))
                return BadRequest(new { message = "Event type not configured." });

            // validate that user is in list of approved users.
            if(!validator.ValidateUser(prEvent.PullRequest.User.Id.ToString()))
                return BadRequest(new { message = "Invalid author." });

            // filter event type from configuration.
            if(!validator.ValidateEventType(prEvent.Action, RepoProvider.Github))
                return BadRequest(new { message = "System is not configured to accept provided event type."});
            
            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent, prEvent.PullRequest.User.Id.ToString());
            return Ok("Successfully posted reviews");
        });
    } 

}