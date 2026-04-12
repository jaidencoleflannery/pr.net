using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using pr.net.Services.Orchestration;
using pr.net.Services.Validations;

using pr.net.Models.Bitbucket;

using static pr.net.Models.Enums.Events;
using static Microsoft.AspNetCore.Http.Results;
using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Endpoints;

public static class BitbucketPullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", async (
            [FromServices] Orchestrator orchestrator,
            [FromServices] IWebhookValidator validator,
            HttpRequest request
        ) => {
            // read body directly as a string so we can encode it and compare to the provided webhook secret.
            using var reader = new StreamReader(request.Body);
            string body = await reader.ReadToEndAsync();

            // validate webhook secret.
            string secretHeader = request.Headers["X-Hub-Signature"].ToString();
            if(string.IsNullOrWhiteSpace(secretHeader) || !await validator.ValidateWebhookSecretAsync(secretHeader, body))
                return BadRequest(new { message = "Unauthorized." });

            BitbucketPullReviewCreatedEventDto prEvent = JsonSerializer.Deserialize<BitbucketPullReviewCreatedEventDto>(body)
                ?? throw new InvalidOperationException("Unexpected error encountered attempting to deserialize request payload."); 

            // validate webhook event type.
            string? eventHeader = request.Headers["X-Event-Key"].ToString();
            if(eventHeader is null || !ValidateEvent(eventHeader, RepoProvider.Bitbucket))
                return BadRequest(new { message = "Event type not configured." });  

            // filter event type from configuration.
            await validator.ValidateEventTypeAsync(eventHeader);

            // logic pipelines.
            await orchestrator.ProcessNewPullRequest(prEvent);
            return Ok("Successfully posted reviews");
        });
    } 

}