using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

namespace pr.net.Models.Tokens;

public class Jwt : Token {

    private string _encodedHeader = string.Empty;
    private string EncodedHeader { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedHeader))
                _encodedHeader = EncodeHeader();
            return _encodedHeader;
        }
    }

    private string _encodedPayload = string.Empty;
    private string EncodedPayload { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedPayload))
                _encodedPayload = EncodePayload();
            return _encodedPayload;
        }
    }

    private JwtHeader _header = new();
    [JsonPropertyName("header")]
    public JwtHeader Header { 
        get => _header; 
        set {
            _header = value;
            _encodedHeader = EncodeHeader();    
        } 
    }

    private JwtPayload _payload = new();
    [JsonPropertyName("payload")]
    public JwtPayload Payload { 
        get => _payload;
        set {
            _payload = value;
            _encodedPayload = EncodePayload();
        } 
    }

    [JsonPropertyName("signature")]
    public string Signature { get; set; }= string.Empty;
    // signature should just be Base64UrlEncode(header.payload)  

    public string Encode(string secret) {
        return $"{EncodedHeader}.{EncodedPayload}.{EncodeSignature(secret)}";
    }

    private string EncodeHeader() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Header));

    private string EncodePayload() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Payload));

    private string? EncodeSignature(string secret) {
        using (RSA rsa = RSA.Create()) {
            if(string.IsNullOrWhiteSpace(secret))
                return null;

            rsa.ImportFromPem(secret);

            string input = $"{EncodedHeader}.{EncodedPayload}";
            var signatureBytes = rsa.SignData(
                Encoding.UTF8.GetBytes(input),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1 // note: github uses rs256.
            );

            return Base64Encode(signatureBytes);
        }
    }

    private static string Base64Encode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

}