namespace pr.net.Services.Tokens;

public class EnvTokenProvider : ITokenProvider { 

    public ValueTask<string> FetchAsync(Token target) =>
        ValueTask.FromResult(System.Environment.GetEnvironmentVariable(Tokens.GetString(target!).ToUpper())
            ?? throw new InvalidOperationException($"{target} environment variable not found.")); 

}