using System.Text.Json.Serialization;

namespace pr.net.Models.Tokens;

public class JwtHeader {

    [JsonPropertyName("alg")]
    public string Alg { get; set; } = string.Empty;

    [JsonPropertyName("typ")]
    public string Typ { get; set; } = string.Empty;

}