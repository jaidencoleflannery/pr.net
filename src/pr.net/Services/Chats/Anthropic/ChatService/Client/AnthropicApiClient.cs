using System.Text;
using System.Text.Json;
using pr.net.Models.Outbound.Anthropic;
using pr.net.Services.Tokens;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming.Anthropic;
using pr.net.Services.Chat.Generic;

namespace pr.net.Services.Chat.Anthropic;

public class AnthropicApiClient(ITokenService tokenService, HttpClient client) : IChatApiClient {

    // RequestReview should be given *individual files* of the cumulative diff, passing the entire diff will reduce the quality of response and should be avoided
    public async Task<List<ChatResponseText>> RequestReviewsAsync(List<AnthropicRequestDto> requestDtos, string url) { 
        // iterate over every instance of requestDtos and send them individually 
        var responses = new List<ChatResponseText>();
        var exceptions = new List<Exception>();
        System.Uri targetUrl = new Uri(url);
        foreach(var requestDto in requestDtos) {
            if(requestDto.Messages.Count < 1)
                continue;
            using (var message = new HttpRequestMessage(HttpMethod.Post, targetUrl)) {
                var token = await tokenService.GetTokenAsync(Token.PR_NET_CHAT_TOKEN); 
                message.Headers.Add("x-api-key", token);
                message.Headers.Add("anthropic-version", "2023-06-01");
                var json = JsonSerializer.Serialize(requestDto);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(message); 
                string responseString = await response.Content.ReadAsStringAsync();
                AnthropicResponseDto responseDto = JsonSerializer.Deserialize<AnthropicResponseDto>(responseString)!;
                AnthropicTextDto textDto = JsonSerializer.Deserialize<AnthropicTextDto>(responseDto.Content[0].Text)!;

                if(response.IsSuccessStatusCode) {
                    if(textDto != null)
                        responses.Add(textDto);
                    else
                        Console.WriteLine("Failed to parse chat response.");
                } else {
                    Exception exception = new Exception($"Request for review failed: {responseDto}");
                    Console.WriteLine(exception);
                    exceptions.Add(exception);
                }
            }
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }

    // UPDATE THIS FUNCTION TO ENSURE THAT THE RETURNED LIST IS IN THE EXACT SAME ORDER AS THE INTAKE LIST
    public async Task<List<ChatResponseText>> RequestFilteringAsync(List<AnthropicRequestDto> requestDtos, string url) {
        // iterate over every instance of requestDtos and send them individually 
        var responses = new List<ChatResponseText>();
        var exceptions = new List<Exception>();
        System.Uri targetUrl = new Uri(url);
        foreach(var requestDto in requestDtos) {
            if(requestDto.Messages.Count < 1)
                continue;
            using (var message = new HttpRequestMessage(HttpMethod.Post, targetUrl)) {
                var token = await tokenService.GetTokenAsync(Token.PR_NET_CHAT_TOKEN); 
                message.Headers.Add("x-api-key", token);
                message.Headers.Add("anthropic-version", "2023-06-01");
                var json = JsonSerializer.Serialize(requestDto);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");                

                var response = await client.SendAsync(message); 
                AnthropicResponseDto responseDto = JsonSerializer.Deserialize<AnthropicResponseDto>(await response.Content.ReadAsStringAsync())!;
                AnthropicFilteringTextDto textDto = JsonSerializer.Deserialize<AnthropicFilteringTextDto>(responseDto.Content[0].Text)!;

                if(response.IsSuccessStatusCode) {
                    if(textDto != null)
                        responses.Add(textDto);
                    else
                        Console.WriteLine("Failed to parse chat response for filtering.");
                } else {
                    Exception exception = new Exception($"Request for filtering failed: {responseDto}");
                    Console.WriteLine(exception);
                    exceptions.Add(exception);
                }
            }
        }

        if(responses.Count > 0)
            return responses;
        else
            throw new HttpRequestException($"No {nameof(RequestReviewsAsync)} calls were successfull, failed to perform review.");
    }

}