using System.Text;
using System.Text.Json;
using pr.net.Models.Incoming.Bitbucket;
using pr.net.Services.Tokens;

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

    // RequestReview should be given a *section* of the diff, passing the entire diff will reduce the quality of response and should be avoided
    public static async Task<List<AnthropicResponseDto>> RequestReviews(HttpClient httpClient, IConfiguration configuration, ITokenService authService, IContextService contextService, Dictionary<string, string> diffSections, int requestId) {

        Provider? provider = ValidateProvider(configuration["Chat:Provider"])
            ?? throw new InvalidOperationException("Configuration for Chat:Provider could not be found or read."); 

        List<string> instructions = await contextService.GetInstructions();
            // ?? throw new InvalidOperationException("Could not fetch instructions.");

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
 
        var requestDtos = new List<AnthropicRequestDto>();
        Console.WriteLine($"\n\n??? OBJ: {JsonSerializer.Serialize(new AnthropicRequestDto(), new JsonSerializerOptions() { WriteIndented = true })}");
        switch(provider) {
            case Provider.Anthropic: 
                if(diffSections == null)
                    throw new InvalidOperationException($"{nameof(diffSections)} was null.");
                var messages = diffSections
                    .Select(diff => new AnthropicMessageDto() { Role = "user", Content = diff.Value })
                    .ToList();
                requestDtos = messages
                    .Select<MessageDto, AnthropicRequestDto>(message => new AnthropicRequestDto() { Model = model, MaxTokens = maxTokens, Messages = messages, OutputConfig = new AnthropicOutputConfig() })
                    .ToList();

                break;

            // currently not configured
            case Provider.OpenAi:

                break;

            // currently not configured
            case Provider.Google:

                break;
        }

        // this will have to be dynamic per dealer - move to switch statement !!!
        // iterate over every instance of requestDtos and send them individually 
        var responses = new List<AnthropicResponseDto>();
        var exceptions = new List<Exception>();
        System.Uri targetUrl = new System.Uri(configuration["Chat:Url"] ?? throw new InvalidOperationException("Unable to find or read Chat:Url from config."));
        foreach(var requestDto in requestDtos) {
            if(requestDto.Messages.Count < 1)
                continue;
            using (var message = new HttpRequestMessage(HttpMethod.Post, targetUrl)) {
                var token = await authService.GetTokenAsync(Token.PR_NET_CHAT_TOKEN); 
                message.Headers.Add("x-api-key", token);
                message.Headers.Add("anthropic-version", "2023-06-01");
                var json = JsonSerializer.Serialize(requestDto);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(message); 
                string responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--- {JsonSerializer.Serialize(new AnthropicResponseContentDto())}");
                Console.WriteLine($"--- {responseJson}");
                AnthropicResponseDto dto = JsonSerializer.Deserialize<AnthropicResponseDto>(responseJson)!;
                if(response.IsSuccessStatusCode) {
                    responses.Add(dto);
                } else {
                    exceptions.Add(new Exception($"Request for review failed for pull review: {requestId}, status code: {response.StatusCode}"));
                }
            }
        }

        if(exceptions.Count > 0)
            foreach(var exception in exceptions)
                Console.WriteLine(exception);
        
        foreach(var response in responses)
            Console.WriteLine($"\n\nRESPONSE: {response.Content[0].Text}\n\n");

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviews)} calls were successfull, failed to perform review.");
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