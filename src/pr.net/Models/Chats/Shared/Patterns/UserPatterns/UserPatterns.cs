using System.Text.Json.Serialization;

namespace pr.net.Models.Patterns;

// all logic for this class needs to exist within the service, this is purely for deserialization and serialization of the file.
public class UserPatterns {

    [JsonPropertyName("userid")]
    public string UserId { get; set; } = string.Empty; // needs to be id, not username.

    [JsonPropertyName("patterns")]
    public List<Pattern> PatternHistory { get; set; } = [];

}