using pr.net.Models.Incoming.Generic;

namespace pr.net.Services.Tokens;

public class EnvTokenProvider : ITokenProvider { 

    public async ValueTask<string> FetchAsync(Token target, PullReviewCreatedEvent? prEvent = null) =>
        await this.FetchAsync(target);

    public ValueTask<string> FetchAsync(Token target) =>
        ValueTask.FromResult(System.Environment.GetEnvironmentVariable(Tokens.GetString(target!).ToUpper())
            ?? throw new InvalidOperationException($"{target} environment variable not found.")); 

}