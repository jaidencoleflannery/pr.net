using static pr.net.Models.Enums.RepoProviders;

namespace pr.net.Models.Enums;

public static class Events {

    // O(1) lookup (TryParse is O(n)).
    public static bool ValidateEvent(string? eventString, RepoProvider provider) {
        if(eventString is null)
            return false;
        switch(provider) {
            case RepoProvider.Github:
                return GithubEventMap.TryGetValue(eventString ?? string.Empty, out _);

            case RepoProvider.Bitbucket:
                return BitbucketEventMap.TryGetValue(eventString ?? string.Empty, out _);
        } 

        return false;
    }

    public static Event StringToEvent(string eventString, RepoProvider provider) {
        if(string.IsNullOrWhiteSpace(eventString))
            return Event.None;

        switch(provider) {
            case RepoProvider.Github:
                GithubEventMap.TryGetValue(eventString.ToLower(), out Event githubResult);
                return githubResult;
            
            case RepoProvider.Bitbucket:
                BitbucketEventMap.TryGetValue(eventString, out Event bitbucketResult);
                return bitbucketResult;

            default:
                return Event.None;
        }
    }

    // these being readonly makes them persist for each call, otherwise these calls will be incredibly slow.  
    private static readonly Dictionary<string, Event> BitbucketEventMap = new() {
            ["pullrequest:created"] = Event.Created,
            ["pullrequest:updated"] = Event.Updated
            // as of 4/11/26, documentation for potential bitbucket event types exists at: https://support.atlassian.com/bitbucket-cloud/docs/event-payloads/
        }; 

    private static readonly Dictionary<string, Event> GithubEventMap = new() {
            ["opened"] = Event.Created,
            ["reopened"] = Event.Updated
            // as of 4/11/26, documentation for potential github event types exists at: https://docs.github.com/en/webhooks/webhook-events-and-payloads
        }; 

        
    public enum Event {
        Created, 
        Updated,
        None
    }

}
