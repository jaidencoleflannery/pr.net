var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/pullrequestcreated", () => ProcessNewPullRequest());

app.Run();
