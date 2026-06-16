using System.Text.Json.Serialization;

using pr.net.Models.Types;

namespace pr.net.Models.Schemas;

public class ItemProperties : IProperties {

    [JsonPropertyName("body")]
    public StringType Body { get; set; } = new(); 

    [JsonPropertyName("line")]
    public IntType Line { get; set; } = new();  

    public List<string> GetRequiredFields() => ["body", "line"];

}