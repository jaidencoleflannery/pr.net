namespace pr.net.Services.Requests;

public interface IRequestService {

    Task<string?> RequestBodyToString(HttpRequest request);

    Task<bool> ValidateRequest(string body);

}

