using System.Text;
using System.Text.Json;
using pr.net.Models.Incoming;
using pr.net.Models.Outbound.Bitbucket;
using pr.net.Services.Tokens;
using pr.net.Services.Instructions;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Clients.Bitbucket;

public static class BitbucketApiClient {
    public static async Task<string> GetPullRequestData(HttpClient httpClient, ITokenService authService, BitbucketPullReviewCreatedMetadataDto request) {
        using(var message = new HttpRequestMessage(HttpMethod.Get, request.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await authService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
            var response = await httpClient.SendAsync(message);

            return (response!= null && response.IsSuccessStatusCode)
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Id}'s data, status: {response?.StatusCode} - {response?.Content}");
        }
    } 

    public static async Task<List<string>> PostReviews(HttpClient httpClient, IConfiguration configuration, ITokenService authService, IContextService contextService, string path, Dictionary<string, string> diffSections, List<AnthropicResponseDto> reviews, RequestPullReviewDto request) {

        var rev = reviews;
        // send each diff file review as it's own individual comment
        var responses = new List<string>();
        var exceptions = new List<Exception>(); 
        for(int index = 0; index < reviews.Count; index++) {
            AnthropicResponseDto review = reviews[index];
            using (var message = new HttpRequestMessage(HttpMethod.Post, $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/comments")) {
                review.Content[0].Text.Content.Inline.Path = path;

                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await authService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));     
                message.Content = new StringContent(JsonSerializer.Serialize(review.Content[index].Text), System.Text.Encoding.UTF8, "application/json");

                Console.WriteLine($"\nCHECKPOINT: {review.Content[index].Text}\n\n");

                Console.WriteLine($"\n\nURL: https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/comments\n\n");

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
                
                var response = await httpClient.SendAsync(message); 
                if(response.IsSuccessStatusCode)
                    responses.Add(await response.Content.ReadAsStringAsync());
                else
                    exceptions.Add(new Exception($"Post for review failed for pull review: {request.Id}, status: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}"));
            }
        }

        if(exceptions.Count > 0)
            foreach(var exception in exceptions)
                Console.WriteLine(exception);

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviews)} calls were successfull, failed to perform review.");
    }

}