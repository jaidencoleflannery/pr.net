using System.Net.Http;
using Serilog;
using pr.net.Models;
using pr.net.Services;
using pr.net.Endpoints;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog(
            (ctx, config) => 
                config
                    .ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.Console()
                    /* .WriteTo.File("logs/pr-.txt", rollingInterval: RollingInterval.Month) we do not need to write to a file when using aws */
        );

        builder.Services.AddSingleton<RequestEngine>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<LocalContextService>();

        var app = builder.Build();

        app.MapPullRequestEndpoints();
        
        app.MapGet("/", () => "Server is running."); 
        app.Run();
    }
}