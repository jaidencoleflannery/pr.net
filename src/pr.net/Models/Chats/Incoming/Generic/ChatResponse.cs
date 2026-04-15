using System.Text.Json.Serialization;

namespace pr.net.Models.Incoming;

public class ChatResponse {

    [JsonPropertyName("content")]
    public List<ChatContent> Content { get; set; } = new(); 

}