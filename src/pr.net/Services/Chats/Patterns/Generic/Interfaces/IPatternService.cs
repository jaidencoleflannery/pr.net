using pr.net.Models.Patterns;

namespace pr.net.Services.Patterns;

public interface IPatternService {

    ValueTask<IEnumerable<Pattern>> TouchUserPatterns(string userId, CancellationToken cancellationToken = default);

    ValueTask<bool> TouchPattern(string userId, int patternId, CancellationToken cancellationToken = default);

    ValueTask<bool> AddPattern(string userId, Pattern pattern, CancellationToken cancellationToken = default);

    ValueTask<string> GetUserPatternsString(string userId, CancellationToken cancellationToken = default);

}