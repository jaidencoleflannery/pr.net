

public class Program {    public static void Main(string[] args) {
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

        CALL FUNCTION_RUN FACT CHECK RUN BACK... RUN

        // dynamic services via configuration - grab config values and validate.

        string? _repoProvider = null; 
        if((_repoProvider = builder.Configuration["Repo:Provider"]) == null)
            throw new InvalidOperationException("Repo:Provider type has not been set in configuration. Set this value before trying again.");
        RepoProvider repoProvider = ValidateRepoProvider(_repoProvider);

        <!-- REMOVE -->

        string? _authProvider = null;
        if((_authProvider = builder.Configuration["Auth:Provider"]) == null)
            throw new InvalidOperationException("Auth:Provider type has not been set in configuration. Set this value before trying again.");
        AuthProvider authProvider = ValidateAuthProvider(_authProvider);

        string? _instructionsProvider = null;
        if((_instructionsProvider = builder.Configuration["Chat:Instructions:Provider"]) == null)
            throw new InvalidOperationException("Chat:Instructions type has not been set in configuration. Set this value before trying again.");
        InstructionsProvider instructionsProvider = ValidateInstructionsPr

        // register services from config values.

        switch(repoProvider) {
            case RepoProvider.Bitbucket: 
                builder.Services.AddHttpClient<IRepositoryApiClient, BitbucketApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { });  
                builder.Services.AddSingleton<ITokenProvider, EnvTokenProvider>();
                break;

            case RepoProvider.Github: 
                builder.Services.AddHttpClient<IRepositoryApiClient, GithubApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { });  
                builder.Services.AddSingleton<ITokenProvider, GithubAppTokenProvider>();
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