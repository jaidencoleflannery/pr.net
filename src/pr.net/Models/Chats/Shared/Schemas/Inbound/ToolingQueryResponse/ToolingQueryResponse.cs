using System.Text.Json.Serialization;

namespace pr.net.Models.Schemas;

public class ToolingQuery {
 
    [JsonPropertyName("runTool")]
    public bool? RunTool { get; set; } = null;

    [JsonPropertyName("toolId")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? ToolId { get; set; } = null;

    [JsonPropertyName("toolInput")]
    public string? ToolInput { get; set; } = null;

}

