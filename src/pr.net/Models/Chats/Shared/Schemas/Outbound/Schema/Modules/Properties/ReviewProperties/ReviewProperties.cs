using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class ReviewProperties : IProperties {

    [JsonPropertyName("reviews")]
    public Reviews Reviews { get; set; } = new(); 

    public List<string> GetRequiredFields() => ["reviews"];

}
