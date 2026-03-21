using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Incoming;
using pr.net.Services.Requests;
using pr.net.Services.Tokens;
using pr.net.Services.Instructions;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] ILogger<BitbucketRequestService> logger, 
            [FromServices] HttpClient httpClient, 
            [FromServices] IConfiguration configuration, 
            [FromServices] ITokenService authService, 
            [FromServices] RequestService requestEngine, 
            [FromServices] IInstructionsService instructionsService,
            [FromBody] PRCreatedEvent prEvent 
            ) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, authService, instructionsService, prEvent));
    } 

}