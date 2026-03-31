using System.Text.Json;

using pr.net.Services.Repositories.Generic;
using pr.net.Services.Tokens;
using pr.net.Services.Context;

using pr.net.Models.Incoming.Anthropic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Github;

namespace pr.net.Services.Clients.Github;

public class GithubApiClient(HttpClient client, ITokenService tokenService, IAmbientContextService<GithubPullReviewCreatedEventDto> _contextService) : IRepositoryApiClient {

    public async Task<string> GetPullRequestDataAsync() {
        GithubPullReviewCreatedEventDto request = _contextService.CreatedEvent;
        if(request is null)
           throw new InvalidOperationException($"AmbientContext could not provide a created event object in {nameof(GithubApiClient)}.");

        using(var message = new HttpRequestMessage(HttpMethod.Get, request.PullRequest?.DiffUrl ?? $"https://github.com/{request.Installation?.Id}/pull/{request.Number}.diff")) {
            string token = await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN); 
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(message);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Number}'s data, status: {response?.StatusCode} - {await response?.Content?.ReadAsStringAsync()!}");
        }
    } 

    public async Task<List<string>> PostReviewsAsync(List<ChatResponseText> reviews) {
        GithubPullReviewCreatedEventDto request = _contextService.CreatedEvent;
        if(request is null)
           throw new InvalidOperationException($"AmbientContext could not provide a created event object in {nameof(GithubApiClient)}."); 

        // send each diff file review as it's own individual comment, and save each status
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        JsonSerializerOptions jsonSettings = new JsonSerializerOptions { IncludeFields = true };
        // THIS NEEDS TO BE GENERIC!
        foreach(AnthropicTextDto review in reviews) {
            using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://github.com/repos/{request.Repository?.Owner}/{request.Repository?.Name}/pulls/{request.PullRequest.Id}/comments")) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
                message.Content = new StringContent(JsonSerializer.Serialize((object)review, jsonSettings), System.Text.Encoding.UTF8, "application/json");

                if(review == null)
                    throw new InvalidOperationException($"{nameof(review)} was null."); 
                
                var response = await client.SendAsync(message); 
                var content = response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                    responses.Add(await response.Content.ReadAsStringAsync());
                else
                    exceptions.Add(new Exception($"Post for review failed for pull review: {request.Number}, status: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}"));
            }
        }

        if(exceptions.Count > 0)
            foreach(var exception in exceptions)
                Console.WriteLine(exception);

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(PostReviewsAsync)} calls were successfull, failed to perform review.");
    }
}