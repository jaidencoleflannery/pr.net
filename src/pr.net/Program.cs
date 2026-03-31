using Serilog;

using pr.net.Services.Chat;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Tokens;
using pr.net.Services.Requests;
using pr.net.Services.Clients.Bitbucket;
using pr.net.Services.Clients.Github;
using pr.net.Services.Repositories.Generic; 
using pr.net.Services.Orchestration;
using pr.net.Services.Chat.Anthropic;
using pr.net.Services.Chat.Generic;
using pr.net.Services.Context;

using pr.net.Endpoints;

using pr.net.Models.Bitbucket;
using pr.net.Models.Github;

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

        // dynamic services via configuration - grab config values and validate

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

        // register services from config values

        switch(repoProvider) {
            case RepoProvider.Bitbucket: 
                builder.Services.AddHttpClient<IRepositoryApiClient, BitbucketApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                        AllowAutoRedirect = false // no redirects, we have to handle them due to auth stripping
                    });  
                break;

            case RepoProvider.Github: 
                builder.Services.AddHttpClient<IRepositoryApiClient, GithubApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                        AllowAutoRedirect = false // no redirects, we have to handle them due to auth stripping
                    });  
                break;
            // chain other repository provider types here - ensure they all have their own request service and apiclient configured
        } 

        switch(authProvider) {
            case AuthProvider.Environment: 
                builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                break;
            
            case AuthProvider.Github:
                builder.Services.AddSingleton<ITokenProvider, GithubAppTokenProvider>();
                break;

            // chain other types of token storage access here
        } 
 
        switch(instructionsProvider) {
            case InstructionsProvider.Environment:
                builder.Services.AddSingleton<IInstructionsService, LocalInstructionsService>();
                break;

            // chain other types of instruction sources here
        } 

        switch(chatProvider) {
            case ChatProvider.Anthropic:
                builder.Services.AddHttpClient<IChatApiClient, AnthropicApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                        AllowAutoRedirect = false // no redirects, we have to handle them due to auth stripping
                    }); 
                builder.Services.AddSingleton<IChatService, AnthropicChatService>();
                break;

            // chain other types of chat providers here
        }

        // generic services
        builder.Services.AddSingleton<Orchestrator>();
        builder.Services.AddSingleton<IRepositoryRequestService, RepositoryRequestService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();  

        var app = builder.Build();
 
        // this endpoint gives you payload examples (dev only)
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
            Console.WriteLine("\npr.net is running in Development mode.\n");
            // leave disabled unless testing
            app.MapIntakeTestingEndpoints(); 
        }  

        switch(repoProvider) {
            case RepoProvider.Bitbucket:  
                builder.Services.AddSingleton<IAmbientContextService<BitbucketPullReviewCreatedEventDto>, BitbucketAmbientContextService>();
                app.MapBitbucketPullRequestEndpoints();
                break;
            
            case RepoProvider.Github:
                builder.Services.AddSingleton<IAmbientContextService<GithubPullReviewCreatedEventDto>, GithubAmbientContextService>();
                app.MapGithubPullRequestEndpoints(); 
                break;

            // chain other types of repo providers and payload model contexts here
        }

        app.MapGet("/", () => $"Server is running in {env} mode."); 

        app.Run();
    }
}