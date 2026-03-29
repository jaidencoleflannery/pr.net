namespace pr.net.Services.Tokens;

public class EnvTokenProvider : ITokenProvider { 

    public virtual ValueTask<string> FetchAsync(Token target) { 
        return ValueTask.FromResult(System.Environment.GetEnvironmentVariable(Tokens.GetString(target!).ToUpper())
            ?? throw new InvalidOperationException($"{target} environment variable not found.")); 
    }

}