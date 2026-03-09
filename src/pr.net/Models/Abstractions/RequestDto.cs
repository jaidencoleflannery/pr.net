using System.Text.Json.Serialization;

namespace pr.net.Models;

public abstract class RequestDto() {

    [JsonPropertyName("messages")]
    public List<MessageDto> Messages = new List<MessageDto>();

}