using Serilog;
using pr.net.Models.Enums;
using pr.net.Services.Instructions;
using pr.net.Services.Tokens;
using pr.net.Services.Clients;
using pr.net.Services.Requests.Bitbucket;
using pr.net.Endpoints;

using static pr.net.Models.Enums.RepoProviders;
using static pr.net.Models.Enums.AuthProviders;
using static pr.net.Models.Enums.InstructionsProviders;
using static pr.net.Models.Enums.ChatProviders;

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
                    if(env == "Production") { /* if not reliant on console logging, configure for provider's logging system. ! should be dynamic via config */ } 
            }
        );

        // no redirects, we have to handle them due to auth stripping
        builder.Services.AddSingleton(_ => new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false })); 

        // dynamic services via configuration

        string? _repoProvider = null; 
        if((_repoProvider = builder.Configuration["Repo:Provider"]) == null)
            throw new InvalidOperationException("Repo:Provider type has not been set in configuration. Set this value before trying again.");
        RepoProvider repoProvider = ValidateRepoProvider(_repoProvider);

        string? _authProvider = null;
        if((_authProvider = builder.Configuration["Auth:Provider"]) == null)
            throw new InvalidOperationException("Auth:Provider type has not been set in configuration. Set this value before trying again.");
        AuthProvider authProvider = ValidateAuthProvider(_authProvider);

        string? _instructionsProvider = null;
        if((_instructionsProvider = builder.Configuration["Chat:Instructions:Provider"]) == null)
            throw new InvalidOperationException("Chat:Instructions type has not been set in configuration. Set this value before trying again.");
        InstructionsProvider instructionsProvider = ValidateInstructionsProvider(_instructionsProvider);

        string? _chatProvider = null;
        if((_chatProvider = builder.Configuration["Chat:Provider"]) == null)
            throw new InvalidOperationException("Chat:Instructions type has not been set in configuration. Set this value before trying again.");
        ChatProvider chatProvider = ValidateChatProvider(_chatProvider);

        switch(repoProvider) {
            case RepoProvider.Bitbucket:
                builder.Services.AddSingleton<BitbucketRequestService>();
                break;

            // chain other repository provider types here
        }

        switch(authProvider) {
            case AuthProvider.Environment:
                builder.Services.AddSingleton<ITokenService, EnvTokenService>();
                break;

            // chain other types of token storage access here
        } 
 
        switch(instructionsProvider) {
            case InstructionsProvider.Environment:
                builder.Services.AddSingleton<IInstructionsService, LocalInstructionsService>();
                break;

            // chain other types of instruction sources here
        } 

        var app = builder.Build();
 
        // this endpoint gives you payload examples (dev only)
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
            Console.WriteLine("pr.net is running in Development mode.");
            // app.MapIntakeTestingEndpoints(); // leave disabled unless testing
        }

        switch(repoProvider) {
            case RepoProvider.Bitbucket:
                app.MapBitbucketPullRequestEndpoints();
                break;

            // chain other endpoint types here
        }
        
        app.MapGet("/", () => $"Server is running in {env} mode."); 
        app.Run();
    }
}