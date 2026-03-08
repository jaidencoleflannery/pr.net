using System.Text;
using System.Text.Json;
using pr.net.Models;

using static pr.net.Models.Providers;

namespace pr.net.Services;

public static class PullRequestApiClient {
    public static async Task<string> GetPullRequestData(HttpClient httpClient, IConfiguration configuration, AuthService authService, RequestPullReviewDto request) {
        using (var message = new HttpRequestMessage(HttpMethod.Get, request.Url ?? $"https://api.bitbucket.org/2.0/repositories/{request.RepoSlug}/pullrequests/{request.Id}/diff")) {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetRepoBearerToken(configuration));
            var response = await httpClient.SendAsync(message);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : throw new Exception($"Failed to get pull review {request.Id}'s data, status: {response.StatusCode} - {response.Content}");
        }
    }

    // RequestReview should be given a *section* of the diff, passing the entire diff will reduce the quality of response and should be avoided
    public static async Task<List<PropertiesDto>> RequestReviews(HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, List<string> diffSections, string requestId) {
        Provider? provider = ValidateProvider(configuration["Chat:Provider"])
            ?? throw new InvalidOperationException("Configuration for Chat:Provider could not be found or read."); 
        List<string> instructions = await contextService.GetInstructions() 
            ?? throw new InvalidOperationException("Could not fetch instructions.");
        string? url = GetUrl(provider.Value)
            ?? throw new InvalidOperationException($"Unexpected error encountered attempting to find string for provider {provider}");
        string model = configuration["Chat:Model"] 
            ?? throw new InvalidOperationException("Configuration for Chat:Model could not be found or read."); 
        string maxTokensString = configuration["Chat:MaxTokens"]
            ?? throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read.");
        if(!int.TryParse(maxTokensString, out var maxTokens))
            throw new InvalidOperationException("Configuration for Chat:MaxTokens could not be found or read, or is in an invalid format.");

        // instructions is a per line array so we can optionally do weird stuff to it in other places
        StringBuilder instructionsBuilder = new StringBuilder();
        foreach(var instruction in instructions)
            instructionsBuilder.AppendLine(instruction);
 
        var requestDtos = new List<RequestDto>();
        switch(provider) {
            case Provider.Anthropic: 
                var messages = diffSections
                    .Select<string, MessageDto>(diff => new AnthropicMessageDto() { Role = "user", Content = diff })
                    .ToList();
                requestDtos = messages
                    .Select<MessageDto, RequestDto>(message => new AnthropicRequestDto() { Model = model, MaxTokens = maxTokens, Messages = messages, System = instructionsBuilder.ToString(), OutputConfig = new AnthropicOutputConfig() })
                    .ToList();

                break;

            // currently not configured
            case Provider.OpenAi:

                break;

            // currently not configured
            case Provider.Google:

                break;
        }

        // iterate over every instance of requestDtos and send them individually 
        var responses = new List<PropertiesDto>();
        var exceptions = new List<Exception>();
        foreach(var requestDto in requestDtos)
            using (var message = new HttpRequestMessage(HttpMethod.Post, configuration["Chat:Url"])) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetChatBearerToken(configuration));     
                message.Content = new StringContent(JsonSerializer.Serialize(requestDto), System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(message); 
                if(response.IsSuccessStatusCode)
                    responses.Add(await response.Content.ReadFromJsonAsync<PropertiesDto>());
                else
                    exceptions.Add(new Exception($"Request for review failed for pull review: {requestId}, status code: {response.StatusCode}"));
            }

        if(exceptions.Count > 0)
            foreach(var exception in exceptions)
                Console.WriteLine(exception);

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviews)} calls were successfull, failed to perform review.");
    }

    public static async Task<List<string>> PostReviews(HttpClient httpClient, IConfiguration configuration, AuthService authService, IContextService contextService, List<PropertiesDto> reviews, string requestId) {
        // send each diff file review as it's own individual comment
        var responses = new List<string>();
        var exceptions = new List<Exception>();
        foreach(var review in reviews) {
            using (var message = new HttpRequestMessage(HttpMethod.Post, configuration["Repo:Url"])) {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authService.GetRepoBearerToken(configuration));     
                message.Content = new StringContent(JsonSerializer.Serialize(review), System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(message); 
                if(response.IsSuccessStatusCode)
                    responses.Add(await response.Content.ReadAsStringAsync());
                else
                    exceptions.Add(new Exception($"Post for review failed for pull review: {requestId}, status code: {response.StatusCode}"));
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