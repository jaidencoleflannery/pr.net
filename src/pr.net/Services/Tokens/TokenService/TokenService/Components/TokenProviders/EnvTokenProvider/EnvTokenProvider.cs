namespace pr.net.Services.Tokens;

public class EnvTokenProvider : ITokenProvider { 

    public ValueTask<string> FetchAsync(string target) {
        return ValueTask.FromResult(System.Environment.GetEnvironmentVariable(target.ToUpper())
            ?? throw new InvalidOperationException($"{target} environment variable not found.")); 
    }

}