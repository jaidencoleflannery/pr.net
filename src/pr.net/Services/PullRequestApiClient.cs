using pr.net.Models;

namespace pr.net.Services;

public static class PullRequestApiClient {
    public static async Task<string> GetPullReviewData(HttpClient httpClient, IConfiguration configuration, AuthService authService, RequestPullReviewDto request) {
        var message = new HttpRequestMessage(HttpMethod.Get, request.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff");
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetRepoBearerToken(configuration));
        var response = await httpClient.SendAsync(message);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : throw new Exception($"Failed to get pull review {request.Id}'s data, status code: {response.StatusCode}");
    }

    public static async Task<string> RequestReview(HttpClient httpClient, IConfiguration configuration, AuthService authService, string diff) {
        var message = new HttpRequestMessage(HttpMethod.Post, configuration["pr.net.ChatUrl"]);
        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetChatBearerToken(configuration));
        message.Content = JsonContent.Create(/* place object to send to anthropic here */);
    }

}