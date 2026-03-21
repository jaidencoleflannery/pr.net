using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Incoming.Bitbucket;
using pr.net.Services.Requests.Bitbucket;
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
            [FromServices] BitbucketRequestService requestEngine, 
            [FromServices] IInstructionsService instructionsService,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent 
            ) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, authService, instructionsService, prEvent));
    } 

}