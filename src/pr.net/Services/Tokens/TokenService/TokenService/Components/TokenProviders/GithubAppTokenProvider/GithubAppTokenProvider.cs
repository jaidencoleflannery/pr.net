using System.Security.Cryptography;
namespace pr.net.Services.Tokens;

public class GithubAppTokenProvider(HttpClient _client) : ITokenProvider { 

    public ValueTask<string> FetchAsync(string target) {
        using (var message = new HttpRequestMessage(HttpMethod.Post, $"")) {

            RSA rsa = RSA.Create();

            var 

            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenService.GetTokenAsync(Token.PR_NET_REPO_TOKEN));
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

}