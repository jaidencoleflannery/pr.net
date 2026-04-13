namespace pr.net.Services.Patterns;

public interface IPatternService {

    Task<List<string>> GetUserPatterns(string userId);

    Task<bool> InitializePatterns();

}