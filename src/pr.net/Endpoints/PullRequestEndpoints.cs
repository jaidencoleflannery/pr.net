using pr.net.Models;
using pr.net.Services;

namespace pr.net.Endpoints;

public static class PullRequestEndpoints {

    public static void MapPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/pullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            ILogger<RequestEngine> logger, HttpClient httpClient, 
            IConfiguration configuration, AuthService authService, 
            RequestEngine requestEngine, IContextService contextService,
            PullRequestDto request
            ) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, authService, contextService, request));
    } 

}