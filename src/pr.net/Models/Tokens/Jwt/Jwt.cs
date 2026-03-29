using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

namespace pr.net.Models.Tokens;

public class Jwt : Token {
    private string _encodedHeader = string.Empty;
    public string EncodedHeader { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedHeader))
                EncodeHeader();
            return _encodedHeader;
        }
    }

    private string _encodedPayload = string.Empty;
    public string EncodedPayload { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedPayload))
                EncodeHeader();
            return _encodedPayload;
        }
    }

    private string _encodedSignature = string.Empty;
    public string EncodedSignature { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedSignature))
                EncodeHeader();
            return _encodedSignature;
        }
    }

    [JsonPropertyName("header")]
    public JwtHeader Header { get; set; } = new();

    [JsonPropertyName("payload")]
    public JwtPayload Payload { get; set; } = new();

    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty; // signature should just be Base64UrlEncode(header.payload)

    public string Encode() =>
        $"{EncodedHeader}.{EncodedPayload}.{EncodedSignature}";

    private string EncodeHeader() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Header));

    private string EncodePayload() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Payload));

    private string EncodeSignature() {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN"));

        string input = $"{EncodeHeader()}.{EncodePayload()}";
        var signatureBytes = rsa.SignData(
            Encoding.UTF8.GetBytes(input),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1 // this will depend on the provider's expectation - github uses rs256 so we're defaulting to that for now
        );

        return Base64Encode(signatureBytes);
    }

    private string Base64Encode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

}