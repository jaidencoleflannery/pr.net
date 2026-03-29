using System.Text.Json.Serialization;

namespace pr.net.Models.Tokens;

public class JwtHeader {

    [JsonPropertyName("alg")]
    public string Alg { get; set; } = "RS256";

    [JsonPropertyName("typ")]
    public string Typ { get; set; } = "JWT";

}