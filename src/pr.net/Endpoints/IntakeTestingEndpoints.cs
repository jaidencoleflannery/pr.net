using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using pr.net.Models.Bitbucket;

namespace pr.net.Endpoints;

public static class IntakeTestingEndpoints {

    public static void MapIntakeTestingEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/development").WithTags("PullRequests");
        group.MapPost("/intaketesting", ( 
            [FromBody] BitbucketPullReviewCreatedEventDto request
            ) => Console.WriteLine(JsonSerializer.Serialize(request, new JsonSerializerOptions() { WriteIndented = true })));
    } 

}