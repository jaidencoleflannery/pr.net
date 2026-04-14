using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class Review {
 
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    [JsonPropertyName("line")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Line { get; set; } = 0;

}