using Amazon.Lambda;
using Amazon.Lambda.Model;
using System.Text.Json;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        string functionName = builder.Configuration["Lambda:FunctionName"]
            ?? throw new InvalidOperationException("Configuration value for Lambda:FunctionName is invalid.");

        string repoProvider = builder.Configuration["Repo:Provider"]
            ?? throw new InvalidOperationException("Configuration value for Repo:Provider is invalid.");

        AmazonLambdaClient lambdaClient = new();

        app.MapPost("{**path}", async (HttpContext context) => {
            using var reader = new StreamReader(context.Request.Body);
            string body = await reader.ReadToEndAsync();

            var queryParams = context.Request.Query
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            var headers = context.Request.Headers
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            var payload = JsonSerializer.Serialize(new {
                httpMethod = context.Request.Method,
                path = context.Request.Path.Value,
                queryStringParameters = queryParams,
                headers = headers,
                body = body,
                isBase64Encoded = false
            });

            _ = lambdaClient.InvokeAsync(new InvokeRequest {
                FunctionName = functionName,
                InvocationType = InvocationType.Event,
                Payload = payload
            });

            return Results.Ok();
        });

        app.Run();
    }
}

