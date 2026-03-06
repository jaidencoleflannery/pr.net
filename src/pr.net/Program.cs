using pr.net.Models;
using pr.net.Services;
using static pr.net.Services.RequestEngine;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        
        var _requestEngine = new RequestEngine();

        app.MapGet("/", () => "Server is running.");
        app.MapGet("/pullrequestcreated", (ILogger<RequestEngine> logger, PullRequestDto request) => _requestEngine.ProcessNewPullRequest(logger, request));

        app.Run();
    }
}