using System.Text.Json.Serialization;

using pr.net.Models.Types;

namespace pr.net.Models.Schemas;

public class ToolingProperties : IProperties {
 
    [JsonPropertyName("runTool")]
    public BooleanType RunTool { get; set; } = new();

    [JsonPropertyName("toolId")]
    public StringType ToolId { get; set; } = new();

    public List<string> GetRequiredFields() => ["runTool"];

}
 
