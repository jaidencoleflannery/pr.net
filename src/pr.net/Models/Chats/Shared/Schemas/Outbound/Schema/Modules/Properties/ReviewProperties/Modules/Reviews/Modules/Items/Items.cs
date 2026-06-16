using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class Items : IProperties {

    public Items() {
        this.Required = [..new ItemProperties().GetRequiredFields()];
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public ItemProperties Properties { get; set; } = new(); 

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false;

    public List<string> GetRequiredFields() => ["properties"];

}