using System.Text.Json;

using pr.net.Services.Repositories.Generic;
using pr.net.Services.Tokens;

using pr.net.Models.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Github;

namespace pr.net.Services.Clients.Github;

public class GithubApiClient(HttpClient _client, ITokenService _tokenService, IConfiguration _configuration) : IRepositoryApiClient {

    public async Task<string> GetPullRequestDataAsync(PullReviewCreatedEvent prEvent) {
        if(prEvent is not GithubPullReviewCreatedEventDto request)
           throw new InvalidOperationException($"Event type did not match injected service type {nameof(GetPullRequestDataAsync)}.");

        string url = (string.IsNullOrWhiteSpace(request.PullRequest?.DiffUrl)
            ? $"https://github.com/{request.Installation?.Id}/pull/{request.Number}.diff" 
            : request.PullRequest?.DiffUrl)!;

        using(var message = new HttpRequestMessage(HttpMethod.Get, url)) {
            string token = await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent: prEvent); 
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.SendAsync(message);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Number}'s data, status: {response?.StatusCode} - {await response?.Content?.ReadAsStringAsync()!}");
        }
    } 

    public async Task<List<string>> PostReviewsAsync(IEnumerable<(DiffSection, ChatResponse)> reviews, PullReviewCreatedEvent prEvent) {
        if(prEvent is not GithubPullReviewCreatedEventDto request)
           throw new InvalidOperationException($"Event type did not match injected service type {nameof(PostReviewsAsync)}.");

        string appName = _configuration["Repo:Github:AppName"] 
            ?? throw new InvalidOperationException("Configuration value Repo:Github:AppName could not be read or is invalid, make sure to set this value before rebooting.");

        // send each diff file review as it's own individual comment, and save each status
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        JsonSerializerOptions jsonSettings = new JsonSerializerOptions { IncludeFields = true };
        foreach(var (diff, review) in reviews) {
            using(var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.github.com/repos/{request.Repository?.Owner.Login}/{request.Repository?.Name}/pulls/{request.Number}/comments")) { 
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent));
                message.Headers.Add("User-Agent", appName);

                if(review == null) {
                    exceptions.Add(new InvalidOperationException($"{nameof(review)} was null.")); 
                    continue;
                }
                GithubComment comment = new() {
                    Body = ((dynamic)review).Content[0].Text,
                    CommitId = ((GithubPullReviewCreatedEventDto)prEvent).PullRequest.Head.Sha,
                    Path = diff.Path,
                    Line = ((dynamic)review).Content[0].Line // needs to be fixed
                };
                message.Content = new StringContent(JsonSerializer.Serialize(comment, jsonSettings), System.Text.Encoding.UTF8, "application/json"); 
                
                var response = await _client.SendAsync(message); 
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