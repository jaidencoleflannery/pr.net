using System.Text.Json;

using pr.net.Services.Repositories.Generic;
using pr.net.Services.Tokens;

using pr.net.Models.Bitbucket;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Generic;

namespace pr.net.Services.Clients.Bitbucket;

public class BitbucketApiClient(HttpClient client, ITokenService _tokenService) : IRepositoryApiClient {

    public async Task<string> GetPullRequestDataAsync(PullReviewCreatedEvent prEvent) {
        if(prEvent is not BitbucketPullReviewCreatedEventDto request)
           throw new InvalidOperationException($"AmbientContext could not provide a created event object in {nameof(BitbucketApiClient)}.");

        BitbucketPullReviewCreatedMetadataDto metadata = new(request);
        using(var message = new HttpRequestMessage(HttpMethod.Get, metadata.Url ?? $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/pullrequests/{metadata.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent));
            var response = await client.SendAsync(message);

            return (response!= null && response.IsSuccessStatusCode)
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {metadata.Id}'s data, status: {response?.StatusCode} - {response?.Content}");
        }
    } 

    public async Task<List<string>> PostReviewsAsync(IEnumerable<(DiffSection, ChatResponse)> reviews, PullReviewCreatedEvent prEvent) {
        if(prEvent is not BitbucketPullReviewCreatedEventDto request)
           throw new InvalidOperationException($"AmbientContext could not provide a created event object in {nameof(BitbucketApiClient)}.");

        BitbucketPullReviewCreatedMetadataDto metadata = new(request); // grab minimum metadata.

        // send each diff file review as it's own individual comment, and save each status.
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        JsonSerializerOptions jsonSettings = new JsonSerializerOptions { IncludeFields = true };
        // FIX THIS DEPENDENCY v
        foreach(var(diff, review) in reviews) {
            using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/pullrequests/{metadata.Id}/comments")) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN, prEvent));
                message.Content = new StringContent(JsonSerializer.Serialize((object)review, jsonSettings), System.Text.Encoding.UTF8, "application/json");

                if(review == null)
                    throw new InvalidOperationException($"{nameof(review)} was null."); 
                
                var response = await client.SendAsync(message); 
                var content = response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                    responses.Add(await response.Content.ReadAsStringAsync());
                else
                    exceptions.Add(new Exception($"Post for review failed for pull review: {metadata.Id}, status: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}"));
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