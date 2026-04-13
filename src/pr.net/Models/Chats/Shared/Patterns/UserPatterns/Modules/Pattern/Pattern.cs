using System.Text.Json.Serialization;

using static pr.net.Models.Enums.PatternSeverities;

namespace pr.net.Models.Patterns;

public class Pattern {

    [JsonPropertyName("severity")]
    public PatternSeverity Severity { get; set; } = PatternSeverity.None;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

}