using System.Text.Json.Serialization;

namespace pr.net.Models.Tokens;

public class JwtPayload {

    [JsonPropertyName("iss")]
    public string Iss { get; set; } = string.Empty;

    [JsonPropertyName("iat")]
    public string Iat { get; set; } = string.Empty;

    [JsonPropertyName("exp")]
    public string Exp { get; set; } = string.Empty;

}