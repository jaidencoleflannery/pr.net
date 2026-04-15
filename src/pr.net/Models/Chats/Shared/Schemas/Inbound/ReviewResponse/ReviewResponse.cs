using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class ReviewResponse {
 
    [JsonPropertyName("reviews")]
    public List<Review>? Reviews { get; set; } = [];

} 