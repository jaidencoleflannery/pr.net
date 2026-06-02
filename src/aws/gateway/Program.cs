public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        string? lambdaUrl = builder.Configuration["Lambda:Url"]
            ?? throw new InvalidOperationException("Configuration value for Lambda:Url is invalid.");

        string? repoProvider = builder.Configuration["Repo:Provider"]
            ?? throw new InvalidOperationException("Configuration value for Repo:Provider is invalid.");

        HttpClient client = new();
        
        app.MapPost("{**path}", async (HttpContext context) => {
            HttpRequestMessage message = new(HttpMethod.Post, lambdaUrl + context.Request.Path + context.Request.QueryString);
            message.Content = new StreamContent(context.Request.Body);

            foreach(var (key, value) in context.Request.Headers)
                message.Headers.TryAddWithoutValidation(key, (IEnumerable<string>)value);

            _ = client.SendAsync(message);

            return Results.Ok();
        });

        app.Run();
    }
}


