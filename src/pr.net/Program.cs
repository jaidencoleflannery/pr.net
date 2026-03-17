using Serilog;
using pr.net.Services.Instructions;
using pr.net.Services.Tokens;
using pr.net.Services.Clients;
using pr.net.Services.Requests;
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
                    if(env == "Production") { /* if not reliant on console logging, configure for provider's logging system */ } 
            }
        );

        builder.Services.AddSingleton<RequestService>();
        // no redirects - we have to handle them due to auth stripping
        builder.Services.AddSingleton<HttpClient>(_ => new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }));
        builder.Services.AddSingleton<TokenService>();
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            builder.Services.AddSingleton<IContextService, LocalContextService>();
        // contextservice will depend on the provider, and will have to be hotswapped here:
        /* else 
            builder.Services.AddSingleton<IContextService, {providerContextService}> */

        var app = builder.Build();

        // this endpoint gives you payload examples
        /*
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
            app.MapTestPullRequestEndpoints(); 
        }
        */
        app.MapPullRequestEndpoints();
        
        app.MapGet("/", () => $"Server is running in {env} mode."); 
        app.Run();
    }
}