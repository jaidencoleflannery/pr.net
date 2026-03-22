namespace pr.net.Services.Chat;

public interface IChatService {
    
    Task GetReviewsAsync(Dictionary<string, string> diffSections);



}