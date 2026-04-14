using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class FilteringResponse {
 
    [JsonPropertyName("isWorthReview")]
    public bool? IsWorthReview { get; set; } = null;

}
     