namespace pr.net.Services.Requests;

public abstract class RequestService : IRequestService {

    // validate requests.
    public async Task<string?> RequestBodyToString(HttpRequest request) { 
        try {
            using StreamReader reader = new(request.Body);
            string body = await reader.ReadToEndAsync();

            return body;
        } catch {
            return null;
        } 
    }

    public abstract Task<bool> ValidateRequest(string body);

}

