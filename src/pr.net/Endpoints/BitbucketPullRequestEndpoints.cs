using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using static Microsoft.AspNetCore.Http.Results;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Bitbucket;

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

public static class BitbucketPullRequestEndpoints {
    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
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
            string secretHeader = request.Headers["X-Hub-Signature"].ToString();
            if(string.IsNullOrWhiteSpace(secretHeader) || !await validator.ValidateWebhookSecretAsync(secretHeader, body))
                return null;

            BitbucketPullReviewCreatedEventDto prEvent = JsonSerializer.Deserialize<BitbucketPullReviewCreatedEventDto>(body)
                ?? throw new InvalidOperationException("Unexpected error encountered attempting to deserialize request payload."); 

            // validate webhook event type.
            string? eventHeader = request.Headers["X-Event-Key"].ToString();
            if(!ValidateEvent(eventHeader, RepoProvider.Bitbucket))
                throw new InvalidOperationException("Event type not configured or invalid, rejecting request.");

            // validate that user is in list of approved users.
            if(!validator.ValidateUser(prEvent.PullRequest.Author.AccountId))
                throw new InvalidOperationException("Author not found on whitelist, rejecting request.");

            // filter event type from configuration.
            if(!validator.ValidateEventType(eventHeader, RepoProvider.Bitbucket))
                throw new InvalidOperationException("System is not configured to accept provided event type, rejecting request.");

            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent, prEvent.PullRequest.Author.AccountId);
            return Ok("Successfully posted reviews.");
        });
    } 
}

