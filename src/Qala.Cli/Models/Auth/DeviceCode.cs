using System.Text.Json.Serialization;

namespace Qala.Cli.Models.Auth;

public class DeviceCode
{
    [JsonPropertyName("device_code")]
    public string Code { get; set; } = string.Empty;
    [JsonPropertyName("user_code")]
    public string UserCode { get; set; } = string.Empty;
    [JsonPropertyName("verification_uri_complete")]
    public string VerificationUriComplete { get; set; } = string.Empty;
    [JsonPropertyName("interval")]
    public int Interval { get; set; }
}