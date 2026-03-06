using Microsoft.AspNetCore.Mvc;
using pr.net.Models;
using pr.net.Services;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] ILogger<RequestEngine> logger, 
            [FromServices] HttpClient httpClient, 
            [FromServices] IConfiguration configuration, 
            [FromServices] AuthService authService, 
            [FromServices] RequestEngine requestEngine, 
            [FromServices] IContextService contextService,
            [FromBody] ClaudeNewPullRequestDto request
            ) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, authService, contextService, request));
    } 

}