using System.Net.Http;
using Serilog;
using pr.net.Models;
using pr.net.Services;
using pr.net.Endpoints;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        builder.Host.UseSerilog(
            (ctx, config) => {
                config
                    .ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.Console();
                    if(env == "Development")
                        config.WriteTo.File("logs/pr-.txt", rollingInterval: RollingInterval.Month);
                    if(env == "Production") { /* configure provider's logging system, if not reliant on console logging */ } 
            }
        );

        builder.Services.AddSingleton<RequestEngine>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<IContextService, LocalContextService>();

        var app = builder.Build();

        app.MapPullRequestEndpoints();
        
        app.MapGet("/", () => $"Server is running in {env} mode."); 
        app.Run();
    }
}