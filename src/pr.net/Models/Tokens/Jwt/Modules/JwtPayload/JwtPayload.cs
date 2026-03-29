using System.Text.Json.Serialization;

namespace pr.net.Models.Tokens;

public class JwtPayload {

    [JsonPropertyName("iss")]
    public string Iss { get; set; } = string.Empty;

    [JsonPropertyName("iat")]
    public long Iat { get; set; } = -1;

    [JsonPropertyName("exp")]
    public long Exp { get; set; } = -1;

}