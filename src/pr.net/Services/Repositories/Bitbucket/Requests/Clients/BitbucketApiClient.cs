using System.Text.Json;
using pr.net.Models.Outbound.Bitbucket;
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

    public async Task<List<string>> PostReviews(List<ChatResponseText> reviews, PullReviewCreatedMetadata request) {
        BitbucketPullReviewCreatedMetadataDto metadata = (BitbucketPullReviewCreatedMetadataDto)request;

        // send each diff file review as it's own individual comment, and save each status
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        for(int index = 0; index < reviews.Count; index++) {
            ChatResponseText review = reviews[index];
            using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.bitbucket.org/2.0/repositories/{metadata.RepoSlug}/pullrequests/{metadata.Id}/comments")) {
                // figure this out!!!!!! review.Content[0].Text.Content.Inline.Path = diffSections[]; <= need to add that to params

                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
                message.Content = new StringContent(JsonSerializer.Serialize(review), System.Text.Encoding.UTF8, "application/json");

                /*
                if(diffSections == null)
                    throw new InvalidOperationException($"{nameof(diffSections)} was null.");
                var messages = diffSections
                    .Select(diff => new AnthropicMessageDto() { Role = "user", Content = diff.Value })
                    .ToList();
                var requestDtos = messages
                    .Select(message => new AnthropicRequestDto() { Model = model, MaxTokens = maxTokens, Messages = messages, OutputConfig = new AnthropicOutputConfig() })
                    .ToList();
                */
                
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