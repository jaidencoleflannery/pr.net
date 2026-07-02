using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using static Microsoft.AspNetCore.Http.Results;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Github;

using static pr.net.Models.Enums.Events;
using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Endpoints;

/*
 * all endpoints act as a gateway, do not log at this scope.
 *
 * due to the fact that repositories like bitbucket and github 
 * will send multiple webhooks if the service does not respond quickly enough,
 * we have to give a fake status code and rely on logging for errors.
 */

// TODO: strip the logic out of all endpoints and combine them.

public static class GithubPullRequestEndpoints {
    public static void MapGithubPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/github/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromServices] IWebhookValidator validator,
            HttpRequest request,
            HttpContext context
        ) => {
            // early return to avoid duplicate repository webhooks.
            context.Response.StatusCode = 200;
            await context.Response.CompleteAsync();

            // read body directly as a string so we can encode it and compare to the provided webhook secret.
            using var reader = new StreamReader(request.Body);
            // this can throw, just let it.
            string body = await reader.ReadToEndAsync();

            // validate webhook secret.
            string secretHeader = request.Headers["X-Hub-Signature-256"].ToString();
            if(!await validator.ValidateWebhookSecretAsync(secretHeader, body))
                return Empty;

            GithubPullReviewCreatedEventDto prEvent = JsonSerializer.Deserialize<GithubPullReviewCreatedEventDto>(body)
                ?? throw new InvalidOperationException("Unexpected error encountered attempting to deserialize request payload."); 

            // validate webhook event type.
            if(!ValidateEvent(prEvent.Action, RepoProvider.Github))
                throw new InvalidOperationException("Event type not configured or invalid, rejecting request.");

            // validate that user is in list of approved users.
            if(!validator.ValidateUser(prEvent.PullRequest.User.Id.ToString()))
                throw new InvalidOperationException("Author not found on whitelist, rejecting request.");

            // filter event type from configuration.
            if(!validator.ValidateEventType(prEvent.Action, RepoProvider.Github))
                throw new InvalidOperationException("System is not configured to accept provided event type, rejecting request.");
            
            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent, prEvent.PullRequest.User.Id.ToString());
            return Ok("Successfully posted reviews.");
        });
    } 
}

