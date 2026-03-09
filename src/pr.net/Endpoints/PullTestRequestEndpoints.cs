using Microsoft.AspNetCore.Mvc;
using pr.net.Models;
using pr.net.Services;

namespace pr.net.Endpoints;

public static class TestPullRequestEndpoints {

    public static void MapTestPullRequestEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/testpullrequest").WithTags("PullRequests");
        group.MapPost("/created", (
            [FromServices] ILogger<RequestEngine> logger, 
            [FromServices] HttpClient httpClient, 
            [FromServices] IConfiguration configuration, 
            [FromServices] AuthService authService, 
            [FromServices] RequestEngine requestEngine, 
            [FromServices] IContextService contextService,
            [FromBody] Object request
            ) => {
                string json = System.Text.Json.JsonSerializer.Serialize(request, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                logger.LogError("{@Request}", json);
            });
    } 

}