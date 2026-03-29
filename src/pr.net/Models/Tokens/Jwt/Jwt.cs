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

    private string _encodedSignature = string.Empty;
    private string EncodedSignature { 
        get {
            if(string.IsNullOrWhiteSpace(_encodedSignature))
                _encodedSignature = EncodeSignature();
            return _encodedSignature;
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

    private string _signature = string.Empty;
    [JsonPropertyName("signature")]
    public string Signature { 
        get => _signature; 
        set {
            _signature = value;
            _encodedSignature = EncodeSignature();
        } 
    } // signature should just be Base64UrlEncode(header.payload) 

    public string Encode() =>
        $"{EncodedHeader}.{EncodedPayload}.{EncodedSignature}";

    private string EncodeHeader() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Header));

    private string EncodePayload() =>
        Base64Encode(JsonSerializer.SerializeToUtf8Bytes(this.Payload));

    private string EncodeSignature() {
        using (RSA rsa = RSA.Create()) {
            string secret = Environment.GetEnvironmentVariable("PR_NET_REPO_TOKEN") ?? string.Empty;
            if(string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Environment variable PR_NET_REPO_TOKEN either does not exist or could not be found.");

            rsa.ImportFromPem(secret);

            string input = $"{EncodedHeader}.{EncodedPayload}";
            var signatureBytes = rsa.SignData(
                Encoding.UTF8.GetBytes(input),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1 // this will depend on the provider's expectation - github uses rs256 so we're defaulting to that for now
            );

            return Base64Encode(signatureBytes);
        }
    }

    private string Base64Encode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

}