using System.Text.Json.Serialization;

namespace pr.net.Models;

public abstract class RequestDto() {

    // make sure to override this field and tag as [JsonPropertyName("<provider expected field name>")] in children 
    public List<MessageDto> Messages = new List<MessageDto>();

}