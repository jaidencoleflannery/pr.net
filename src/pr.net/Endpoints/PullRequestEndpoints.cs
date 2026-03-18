using Microsoft.AspNetCore.Mvc;
using pr.net.Models;
using pr.net.Services.Requests;
using pr.net.Services.Tokens;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] ILogger<RequestService> logger, 
            [FromServices] HttpClient httpClient, 
            [FromServices] IConfiguration configuration, 
            [FromServices] ITokenService authService, 
            [FromServices] RequestService requestEngine, 
            [FromServices] IContextService contextService,
            [FromBody] NewPullRequestDto request
            ) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, authService, contextService, request));
    } 

}