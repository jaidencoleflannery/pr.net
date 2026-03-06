using System.Net.Http;
using System.Threading.Tasks;
using pr.net.Models;

namespace pr.net.Services;

public static class PullRequestApiClient {
    public static async Task<string> GetPullReviewData(HttpClient httpClient, IConfiguration configuration, RequestPullReviewDto content) {
        var request = new HttpRequestMessage(HttpMethod.Get, content.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthService.GetBearerToken());
        var response = await httpClient.PostAsJsonAsync(, request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : throw new Exception($"Failed to get pull review {request.Id}'s data, status code: {response.StatusCode}");
    }

    public static async Task<string> RequestReview(HttpClient httpClient, )

}