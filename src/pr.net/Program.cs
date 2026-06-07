using Serilog;
using Anthropic;
using Amazon.SecretsManager;
using Amazon.BedrockRuntime;

using pr.net.Services.Chat;
using pr.net.Services.Chat.Instructions;
using pr.net.Services.Tokens;
using pr.net.Services.Requests;
using pr.net.Services.Clients.Bitbucket;
using pr.net.Services.Clients.Github;
using pr.net.Services.Repositories.Generic; 
using pr.net.Services.Orchestration;
using pr.net.Services.Validations;
using pr.net.Services.Tooling;
using pr.net.Services.Tooling.Environment;

using pr.net.Configurations.Host;
using pr.net.Configurations.Chat;
using pr.net.Configurations.Repo;
using pr.net.Configurations.Auth;
using pr.net.Configurations.Tooling;

using pr.net.Endpoints;

using pr.net.Models.Schemas;

using static pr.net.Models.Enums.HostProviders;
using static pr.net.Models.Enums.TokenProviders;
using static pr.net.Models.Enums.RepoProviders;
using static pr.net.Models.Enums.InstructionsProviders;
using static pr.net.Models.Enums.ChatProviders;

namespace pr.net;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if(string.IsNullOrWhiteSpace(env))
            env = "Default";
        builder.Host.UseSerilog(
            (ctx, config) => {
                config
                    .ReadFrom.Configuration(ctx.Configuration)
                    .WriteTo.Console();
                    if(env == "Development")
                        config.WriteTo.File("Logs/pr-.txt", rollingInterval: RollingInterval.Day); 
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

        builder.Services.AddOptions<HostConfiguration>()
            .Bind(builder.Configuration.GetSection("Host"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<ToolingConfiguration>()
            .Bind(builder.Configuration.GetSection("Tooling"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // one time fetch for injection maps.
        HostProvider hostProvider = ValidateHostProvider(builder.Configuration["Host:Provider"]);
        TokenProvider tokenProvider = ValidateTokenProvider(builder.Configuration["Host:TokenProvider"]);
        RepoProvider repoProvider = ValidateRepoProvider(builder.Configuration["Repo:Provider"]);
        InstructionsProvider instructionsProvider = ValidateInstructionsProvider(builder.Configuration["Chat:Instructions:Provider"]);
        ChatProvider chatProvider = ValidateChatProvider(builder.Configuration["Chat:Provider"]);

        // register services from config values - unfortunately cannot use the builder options due to lazy loading.

        switch(hostProvider) {
            case HostProvider.Amazon:
                // the aws sdk handles httpclient, leave as a singleton here.
                // this needs to be fixed - auth is per config, not per host.
                builder.Services.AddSingleton<IAmazonSecretsManager, AmazonSecretsManagerClient>();
                builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
                // token fetcher - source configured from appsettings.
                if(tokenProvider == TokenProvider.AmazonSecretsManager)
                    builder.Services.AddSingleton<ITokenProvider, AmazonTokenProvider>();
                else
                    builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                break;

            case HostProvider.Azure:
                builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                break;


            case HostProvider.Environment:
                // token fetcher - envtokenprovider is the default.
                builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                break;
        }

        switch(repoProvider) {
            case RepoProvider.Bitbucket: 
                // token middleware to augment stored value to the provider's specification - repotokenhandler is the default.
                builder.Services.AddSingleton<IRepoTokenHandler, RepoTokenHandler>();
                // webhook token fetcher.
                builder.Services.AddSingleton<IWebhookSecretHandler, WebhookSecretHandler>(); 
                builder.Services.AddHttpClient<IRepositoryApiClient, BitbucketApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { });   
                break;

            case RepoProvider.Github: 
                // token middleware to augment stored value to the provider's specification.
                builder.Services.AddSingleton<IRepoTokenHandler, GithubAppTokenHandler>();
                // webhook token fetcher.
                builder.Services.AddSingleton<IWebhookSecretHandler, WebhookSecretHandler>(); 
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
                // token source - chattokenhandler is the generic, no special behavior option.
                builder.Services.AddSingleton<IChatTokenHandler, ChatTokenHandler>();
                // the anthropic sdk handles httpclient, leave as a singleton here.
                builder.Services.AddSingleton<IAnthropicClient>(new AnthropicClient() {
                    ApiKey = Environment.GetEnvironmentVariable("PR_NET_CHAT_TOKEN") 
                        ?? throw new InvalidOperationException("Environment variable PR_NET_CHAT_TOKEN could not be found or read, or is in an invalid format.")
                }); 
                builder.Services.AddScoped<IChatClient, AnthropicChatClient>();
                break;

            case ChatProvider.Amazon:
                // token source - chattokenhandler is the generic, no special behavior option.
                builder.Services.AddSingleton<IChatTokenHandler, ChatTokenHandler>();
                // the amazon sdk handles httpclient, leave as a singleton here.
                // the amazon sdk resolves the token from AWS_BEARER_TOKEN_BEDROCK in the environment - if you're running locally, set the variable in your shell.
                builder.Services.AddSingleton<IAmazonBedrockRuntime, AmazonBedrockRuntimeClient>();
                builder.Services.AddScoped<IChatClient, AmazonChatClient>();
                break;

            // chain other types of chat providers here.
        }

        // generic services.
        builder.Services.AddScoped<Orchestrator>();
        builder.Services.AddScoped<IChatService, ChatService>(); 
        builder.Services.AddScoped<IRepositoryRequestService, RepositoryRequestService>();
        builder.Services.AddScoped<IToolChainService, EnvironmentToolChainService>();

        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IWebhookValidator, WebhookValidator>(); 
        // schemas to format ai output.
        builder.Services.AddSingleton<IReviewSchema, Schema<ReviewProperties>>();
        builder.Services.AddSingleton<IFilteringSchema, Schema<FilteringProperties>>();
        // patterns have not been implemented - need to figure out a sound strategy and convert them into a format that can be better analyzed.
        // builder.Services.AddSingleton<IPatternService, LocalPatternService>(); 

        WebApplication app = builder.Build();
        app.MapGet("/", () => $"Server is running in {env} mode."); 
 
        /*
        // this endpoint gives you payload examples (dev only).
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            app.MapIntakeTestingEndpoints(); 
        */

        switch(repoProvider) {
            case RepoProvider.Bitbucket:  
                app.MapBitbucketPullRequestEndpoints();
                break;
            
            case RepoProvider.Github:
                app.MapGithubPullRequestEndpoints(); 
                break;

            // chain other types of repo provider endpoints here.
        } 

        Console.WriteLine(
            @$"  
            {'\u2873'}{'\u28F6'}{'\u28A5'}{'\u282E'} is running in {env} mode.

            | Configuration |
            | * Host:       | [{hostProvider}]
            | * Repository: | [{repoProvider}]
            | * Chat:       | [{chatProvider}]
            "
        );

        app.Run();
    }
}
