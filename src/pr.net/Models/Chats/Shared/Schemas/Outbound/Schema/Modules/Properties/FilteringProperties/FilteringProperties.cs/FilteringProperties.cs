using System.Text.Json.Serialization;

using pr.net.Models.Types;

namespace pr.net.Models.Schemas;

public class FilteringProperties : IProperties {
 
    [JsonPropertyName("isWorthReview")]
    public BooleanType IsWorthReview { get; set; } = new();

    public List<string> GetRequiredFields() => ["isWorthReview"];

}
 