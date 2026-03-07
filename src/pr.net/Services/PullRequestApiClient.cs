using System.Text;
using System.Text.Json;
using pr.net.Models;

using static pr.net.Models.Providers;

namespace pr.net.Services;

public static class PullRequestApiClient {
    public static async Task<string> GetPullReviewData(HttpClient httpClient, IConfiguration configuration, AuthService authService, RequestPullReviewDto request) {
        using (var message = new HttpRequestMessage(HttpMethod.Get, request.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetRepoBearerToken(configuration));
            var response = await httpClient.SendAsync(message);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Id}'s data, status: {response.StatusCode} - {response.Content}");
        }
    }

    public static async Task<string> RequestReview(HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, string diff, RequestPullReviewDto request) {
        List<string> instructions = await contextService.GetInstructions() 
            ?? throw new InvalidOperationException("Could not fetch instructions.");
        Provider? provider = ValidateProvider(configuration["Chat:Provider"])
            ?? throw new InvalidOperationException("Configuration for Chat:Provider could not be found or read."); 
        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}");
        string model = configuration["Chat:Model"] 
            ?? throw new InvalidOperationException("Configuration for Chat:Model could not be found or read."); 

        // instructions is a per line array so we can do optionally do weird stuff to it in other places
        StringBuilder instructionsBuilder = new StringBuilder();
        foreach(var instruction in instructions)
            instructionsBuilder.AppendLine(instruction);
 
        var messages = new List<MessageDto>();
        switch(provider) {
                case Provider.Anthropic: 
                    messages.Add(new AnthropicMessageDto() { Role = "user", Content = $"{diff}"});
                    var json = new ClaudeRequestDto() {
                        Model = model,
                        MaxTokens = 1024,
                        System = instructionsBuilder.ToString(),
                        Messages = messages 
                    };
                    break;

                // currently not configured
                case Provider.OpenAi:
                    url = "https://api.openai.com/v1/responses";
                    break;

                // currently not configured
                case Provider.Google:
                    break;

        }

        using (var message = new HttpRequestMessage(HttpMethod.Post, configuration["pr.net.ChatUrl"])) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetChatBearerToken(configuration));

             
            message.Content = new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(message);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Request for review failed for pull review: {request.Id}, status code: {response.StatusCode}");
        }
    }

}