using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class Reviews : IProperties {

    [JsonPropertyName("type")]
    public string Type { get; set; } = "array";

    [JsonPropertyName("items")]
    public Items Items { get; set; } = new();  

    public List<string> GetRequiredFields() => ["items"];

}