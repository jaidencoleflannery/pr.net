using Serilog;
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
                        config.WriteTo.File("logs/pr-.txt", rollingInterval: RollingInterval.Day);
                    if(env == "Production") { /* if not reliant on console logging, configure provider's logging system */ } 
            }
        );

        builder.Services.AddSingleton<RequestEngine>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<AuthService>();
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            builder.Services.AddSingleton<IContextService, LocalContextService>();
        // contextservice will depend on the provider, and will have to be hotswapped here:
        // else 
        //      builder.Services.AddSingleton<IContextService, {providerContextService}>

        var app = builder.Build();

        app.MapPullRequestEndpoints();
        
        app.MapGet("/", () => $"Server is running in {env} mode."); 
        app.Run();
    }
}