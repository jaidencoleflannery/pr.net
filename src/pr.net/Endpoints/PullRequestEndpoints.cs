using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Incoming.Bitbucket;
using pr.net.Services.Requests.Bitbucket;
using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Chat;
using pr.net.Services.Repositories.Generic;
using pr.net.Services.Orchestration;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] Orchestrator orchestrator,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent
        ) => orchestrator.ProcessNewPullRequest(prEvent));
    } 

}