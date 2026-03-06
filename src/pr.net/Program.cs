using System.Net.Http;
using Serilog;
using pr.net.Models;
using pr.net.Services;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog(
            (ctx, config) => 
                config
                    .ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.Console()
                    .WriteTo.File("logs/pr-.txt", rollingInterval: RollingInterval.Day)
        );

        builder.Services.AddSingleton<RequestEngine>();
        builder.Services.AddSingleton<HttpClient>();

        var app = builder.Build();
        
        app.MapGet("/", () => "Server is running.");
        app.MapPost("/pullrequestcreated", (ILogger<RequestEngine> logger, HttpClient httpClient, IConfiguration configuration, RequestEngine requestEngine, PullRequestDto request) => requestEngine.ProcessNewPullRequest(logger, httpClient, configuration, request));

        app.Run();
    }
}