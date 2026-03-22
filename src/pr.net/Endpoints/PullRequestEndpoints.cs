using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Incoming.Bitbucket;
using pr.net.Services.Requests.Bitbucket;
using pr.net.Services.Tokens;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Chat;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapBitbucketPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/bitbucket/pullrequest").WithTags("PullRequests");
        Console.WriteLine("Endpoint opened at: \"/bitbucket/pullrequest/created\"");
        group.MapPost("/created", (
            [FromServices] ILogger<BitbucketRequestService> logger, 
            [FromServices] IConfiguration configuration, 
            [FromServices] ITokenService tokenService, 
            [FromServices] BitbucketRequestService requestEngine, 
            [FromServices] IInstructionsService instructionsService,
            [FromServices] IChatService chatService,
            [FromBody] BitbucketPullReviewCreatedEventDto prEvent 
            ) => requestEngine.ProcessNewPullRequest(logger, configuration, tokenService, instructionsService, chatService, prEvent));
    } 

}