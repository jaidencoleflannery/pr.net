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

using pr.net.Configurations.Chat;
using pr.net.Configurations.Repo;
using pr.net.Configurations.Auth;

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

        // dynamic services via configuration - grab config values and validate within configuration class.

        builder.Services.AddOptions<ChatConfiguration>()
            .Bind(builder.Configuration.GetSection("Chat"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<RepoConfiguration>()
            .Bind(builder.Configuration.GetSection("Repo"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<AuthConfiguration>()
            .Bind(builder.Configuration.GetSection("Auth"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // one time fetch for injection maps.
        RepoProvider repoProvider = ValidateRepoProvider(builder.Configuration["Repo:Provider"]);
        InstructionsProvider instructionsProvider = ValidateInstructionsProvider(builder.Configuration["Chat:Instructions:Provider"]);
        ChatProvider chatProvider = ValidateChatProvider(builder.Configuration["Chat:Provider"]);

        // register services from config values - unfortunately cannot use the builder options due to lazy loading.

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

                builder.Services.AddSingleton<IAnthropicReviewSchema, AnthropicSchema<AnthropicReviewProperties>>();
                builder.Services.AddSingleton<IAnthropicFilteringSchema, AnthropicSchema<AnthropicGithubFilteringProperties>>();
                // the anthropic sdk handles httpclient, leave as a singleton here.
                builder.Services.AddSingleton<IAnthropicClient>(new AnthropicClient() {
                    ApiKey = Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN") 
                        ?? throw new InvalidOperationException("Environment variable PR_NET_CHAT_TOKEN could not be found or read, or is in an invalid format.")
                }); 

                builder.Services.AddScoped<IChatService, AnthropicChatService>();
                break;

            case ChatProvider.Amazon:
                builder.Services.AddSingleton<IAmazonReviewSchema, AmazonSchema<AmazonReviewProperties>>();
                builder.Services.AddSingleton<IAmazonFilteringSchema, AmazonSchema<AmazonFilteringProperties>>();
                // amazon client here ?
                builder.Services.AddScoped<IChatService, AmazonChatService>();
                break;

            // chain other types of chat providers here.
        }

        // generic services.
        builder.Services.AddScoped<Orchestrator>();
        builder.Services.AddScoped<IRepositoryRequestService, RepositoryRequestService>();
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