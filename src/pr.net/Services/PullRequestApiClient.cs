using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using pr.net.Models;

namespace pr.net.Services;

public static class PullRequestApiClient {
    public static async Task<string> GetPullReviewData(HttpClient httpClient, IConfiguration configuration, AuthService authService, ClaudeRequestPullReviewDto request) {
        using (var message = new HttpRequestMessage(HttpMethod.Get, request.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetRepoBearerToken(configuration));
            var response = await httpClient.SendAsync(message);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Id}'s data, status code: {response.StatusCode}");
        }
    }

    public static async Task<string> RequestReview(HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, string diff, ClaudeRequestPullReviewDto request) {
        List<string> instructions = await contextService.GetInstructions();
        using (var message = new HttpRequestMessage(HttpMethod.Post, configuration["pr.net.ChatUrl"])) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetChatBearerToken(configuration));
            StringBuilder instructionsBuilder = new StringBuilder();
            foreach(var instruction in instructions)
                instructionsBuilder.AppendLine(instruction);
            var messages = new List<MessageDto>() { new MessageDto() { Role = "user", Content = $"You are doing a code review on this PR. Be extremely detailed. Here is the diff: {diff}"} };
            var json = new ClaudeRequestDto() {
                Model = "claude-sonnet-4-20250514",
                MaxTokens = 1024,
                System = instructionsBuilder.ToString(),
                Messages = messages 
            };
            message.Content = new StringContent(JsonSerializer.Serialize(json), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(message);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Request for review failed for pull review: {request.Id}, status code: {response.StatusCode}");
        }
    }

}