using System.Text.Json;
using pr.net.Models.Bitbucket;
using pr.net.Services.Repositories.Generic;
using pr.net.Models.Outbound.Generic;
using pr.net.Models.Incoming.Generic;
using pr.net.Services.Tokens;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Clients.Bitbucket;

public class BitbucketApiClient(HttpClient client, ITokenService tokenService) : IRepositoryApiClient {

    public async Task<string> GetPullRequestData(PullReviewCreatedMetadata request) {
        BitbucketPullReviewCreatedMetadataDto metadata = (BitbucketPullReviewCreatedMetadataDto)request;
        using(var message = new HttpRequestMessage(HttpMethod.Get, metadata.Url ?? $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/pullrequests/{metadata.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
            var response = await client.SendAsync(message);

            return (response!= null && response.IsSuccessStatusCode)
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {metadata.Id}'s data, status: {response?.StatusCode} - {response?.Content}");
        }
    } 

    public async Task<List<string>> PostReviews(Dictionary<string, ChatResponseText> reviews, PullReviewCreatedMetadata request) {
        BitbucketPullReviewCreatedMetadataDto metadata = (BitbucketPullReviewCreatedMetadataDto)request;

        // send each diff file review as it's own individual comment, and save each status
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        JsonSerializerOptions jsonSettings = new JsonSerializerOptions { IncludeFields = true };
        foreach(var (file, review) in reviews) {
            Console.WriteLine($"{file}: {JsonSerializer.Serialize((object)review, jsonSettings)}");
        }
        foreach(var (file, review) in reviews) {
            using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/pullrequests/{metadata.Id}/comments")) {
                Console.WriteLine($"\n{JsonSerializer.Serialize(review)}\n");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
                Console.WriteLine(await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
                message.Content = new StringContent(JsonSerializer.Serialize((object)review, jsonSettings), System.Text.Encoding.UTF8, "application/json");

                if(review == null)
                    throw new InvalidOperationException($"{nameof(review)} was null."); 
                
                var response = await client.SendAsync(message); 
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
            throw new HttpRequestException($"No {nameof(PostReviews)} calls were successfull, failed to perform review.");
    }

}