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
    public async Task<List<ChatResponse>> RequestReviewsAsync(List<AnthropicRequestDto> requestDtos, string url) { 

        // iterate over every instance of requestDtos and send them individually 
        var responses = new List<ChatResponse>();
        var exceptions = new List<Exception>();
        System.Uri targetUrl = new System.Uri(url);
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
                string responseJson = await response.Content.ReadAsStringAsync();
                ChatResponse dto = JsonSerializer.Deserialize<AnthropicResponseDto>(responseJson)!;
                if(response.IsSuccessStatusCode) {
                    responses.Add(dto);
                } else {
                    Exception exception = new Exception($"Request for review failed: {responseJson}");
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