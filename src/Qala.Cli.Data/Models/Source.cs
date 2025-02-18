using System.Text.Json.Serialization;
using Qala.Cli.Data.Utils;

namespace Qala.Cli.Data.Models;

public class Source
{
    public Guid SourceId { get; set; }
    public Guid EnvironmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SourceType SourceType { get; set; }
    [JsonConverter(typeof(SourceConfigurationConverter))]
    public SourceConfiguration Configuration { get; set; } = new();
}

public class SourceConfiguration
{
    public List<HttpMethod> AllowedHttpMethods { get; set; } = [];
    [JsonConverter(typeof(AuthenticationSchemeConverter))]
    public AuthenticationScheme AuthenticationScheme { get; set; } = new NoAuthenticationScheme();
    public List<string> WhitelistedIpRanges { get; set; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SourceType
{
    Http,
    Unknown
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod
{
    GET,
    POST,
    PUT,
    DELETE
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthSchemeType
{
    None,
    Basic,
    ApiKey,
    JWT
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Algorithm
{
    RSA,
    HSA
}

public abstract class AuthenticationScheme
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AuthSchemeType Type { get; set; }
}

public class NoAuthenticationScheme : AuthenticationScheme
{
    public NoAuthenticationScheme()
    {
        Type = AuthSchemeType.None;
    }
}

public class BasicAuthenticationScheme : AuthenticationScheme
{
    public BasicAuthenticationScheme()
    {
        Type = AuthSchemeType.Basic;
    }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SecretType { get; set; } = "secret";
}

public class ApiKeyAuthenticationScheme : AuthenticationScheme
{
    public ApiKeyAuthenticationScheme()
    {
        Type = AuthSchemeType.ApiKey;
    }

    public string ApiKeyName { get; set; } = string.Empty;
    public string ApiKeyValue { get; set; } = string.Empty;
    public string SecretType { get; set; } = "secret";
}

public class JwtAuthenticationScheme : AuthenticationScheme
{
    public JwtAuthenticationScheme()
    {
        Type = AuthSchemeType.JWT;
    }

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public Algorithm Algorithm { get; set; }
    public string SecretType { get; set; } = "cert";
}