using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class Schema<TProperties> : IReviewSchema, IFilteringSchema where TProperties : IProperties, new() {

    public Schema() {
        this.Required = [..Properties.GetRequiredFields()];
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public TProperties Properties { get; set; } = new();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; } = false; 

}