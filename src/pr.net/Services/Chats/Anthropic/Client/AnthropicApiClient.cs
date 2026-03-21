using System.Text;
using System.Text.Json;
using pr.net.Services.Tokens;
using pr.net.Services.Instructions;
using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming.Anthropic;
using pr.net.Models.Outbound.Anthropic;

using static pr.net.Models.Enums.ChatProviders;

namespace pr.net.Services.Chat.Anthropic;

public static class AnthropicApiClient {

    // RequestReview should be given a *section* of the diff, passing the entire diff will reduce the quality of response and should be avoided
    public static async Task<List<ChatResponse>> RequestReviews(HttpClient httpClient, IConfiguration configuration, ITokenService authService, IInstructionsService instructionsService, Dictionary<string, string> diffSections, int requestId) {

        ChatProvider? provider = ValidateChatProvider(configuration["Chat:Provider"]);

        List<string> instructions = await instructionsService.GetInstructions(configuration["Chat:Provider"] ?? string.Empty);
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
            case ChatProvider.Anthropic: 
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

}