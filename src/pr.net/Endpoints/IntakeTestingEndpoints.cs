using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using pr.net.Models;
using pr.net.Services.Requests;
using pr.net.Services.Tokens;

namespace pr.net.Endpoints;

public static class IntakeTestingEndpoints {

    public static void MapIntakeTestingEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/development").WithTags("PullRequests");
        group.MapPost("/intaketesting", (
            [FromServices] ILogger<RequestService> logger, 
            [FromServices] HttpClient httpClient, 
            [FromServices] IConfiguration configuration, 
            [FromServices] ITokenService authService, 
            [FromServices] RequestService requestEngine, 
            [FromServices] IContextService contextService,
            [FromBody] NewPullRequestDto request
            ) => Console.WriteLine(JsonSerializer.Serialize(request, new JsonSerializerOptions() { WriteIndented = true })));
    } 

}