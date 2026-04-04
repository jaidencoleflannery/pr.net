using Serilog;

using Anthropic;

using pr.net.Services.Chat;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Tokens;
using pr.net.Services.Requests;
using pr.net.Services.Clients.Bitbucket;
using pr.net.Services.Clients.Github;
using pr.net.Services.Repositories.Generic; 
using pr.net.Services.Orchestration;

using pr.net.Endpoints;

using pr.net.Models.Anthropic;

using static pr.net.Models.Enums.RepoProviders;
using static pr.net.Models.Enums.AuthProviders;
using static pr.net.Models.Enums.InstructionsProviders;
using static pr.net.Models.Enums.ChatProviders;

namespace pr.net;

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

        // dynamic services via configuration - grab config values and validate.

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

        // register services from config values.

        switch(repoProvider) {
            case RepoProvider.Bitbucket: 
                builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                builder.Services.AddHttpClient<IRepositoryApiClient, BitbucketApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { });   
                break;

            case RepoProvider.Github: 
                builder.Services.AddSingleton<ITokenProvider, GithubAppTokenProvider>();
                builder.Services.AddHttpClient<IRepositoryApiClient, GithubApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { });  
                Console.WriteLine("GithubAppTokenProvider injected."); 
                break;

            // chain other repository provider types here - ensure they all have their own request service and apiclient configured.
            // chain other types of token storage access here.
        }
 
        switch(instructionsProvider) {
            case InstructionsProvider.Environment:
                builder.Services.AddSingleton<IInstructionsService, LocalInstructionsService>();
                break;

            // chain other types of instruction sources here.
        } 

        switch(chatProvider) {
            case ChatProvider.Anthropic:
                switch(repoProvider) {
                    case RepoProvider.Github:
                        builder.Services.AddSingleton<IAnthropicReviewSchema, AnthropicSchema<AnthropicGithubReviewProperties>>();
                        builder.Services.AddSingleton<IAnthropicReviewSchema, AnthropicSchema<AnthropicGithubFilteringProperties>>();
                        break;
                }
                builder.Services.AddSingleton<IAnthropicClient>(new AnthropicClient() {
                    ApiKey = Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN") 
                        ?? throw new InvalidOperationException("Environment variable PR_NET_CHAT_TOKEN could not be found or read, or is in an invalid format.")
                });
                builder.Services.AddSingleton<IChatService, AnthropicChatService>();
                break;

            // chain other types of chat providers here.
        }

        string? _chatTimeoutString = null;
        if((_chatTimeoutString = builder.Configuration["Chat:Timeout"]) == null)
            throw new InvalidOperationException("Chat:Timeout has not been set in configuration. Set this value before trying again.");
        if(!long.TryParse(_chatTimeoutString, out long _chatTimeout))
            throw new InvalidOperationException("Chat:Timeout is invalid, it must be an integer (long). Set this value before trying again.");

        // generic services.
        builder.Services.AddSingleton<Orchestrator>();
        builder.Services.AddSingleton<IRepositoryRequestService, RepositoryRequestService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();

        var app = builder.Build();
        app.MapGet("/", () => $"Server is running in {env} mode."); 
 
        // this endpoint gives you payload examples (dev only).
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            app.MapIntakeTestingEndpoints(); 

        switch(repoProvider) {
            case RepoProvider.Bitbucket:  
                app.MapBitbucketPullRequestEndpoints();
                break;
            
            case RepoProvider.Github:
                app.MapGithubPullRequestEndpoints(); 
                break;

            // chain other types of repo provider endpoints here.
        } 

        app.Run();
    }
}