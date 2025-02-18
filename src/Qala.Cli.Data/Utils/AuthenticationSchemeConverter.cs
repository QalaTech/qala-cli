using System.Text.Json;
using System.Text.Json.Serialization;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Utils;

public class AuthenticationSchemeConverter : JsonConverter<AuthenticationScheme>
{
    public override AuthenticationScheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            JsonElement root = doc.RootElement;
            string type = root.TryGetProperty("type", out JsonElement typeElement) && typeElement.GetString() is string typeString
                ? typeString
                : throw new JsonException("The 'Type' property is missing or null.");

            return type switch
            {
                "None" => JsonSerializer.Deserialize<NoAuthenticationScheme>(root.GetRawText(), options)
                          ?? throw new JsonException("Deserialization of NoAuthenticationScheme returned null."),
                "Basic" => JsonSerializer.Deserialize<BasicAuthenticationScheme>(root.GetRawText(), options)
                           ?? throw new JsonException("Deserialization of BasicAuthenticationScheme returned null."),
                "ApiKey" => JsonSerializer.Deserialize<ApiKeyAuthenticationScheme>(root.GetRawText(), options)
                            ?? throw new JsonException("Deserialization of ApiKeyAuthenticationScheme returned null."),
                "JWT" => JsonSerializer.Deserialize<JwtAuthenticationScheme>(root.GetRawText(), options)
                         ?? throw new JsonException("Deserialization of JwtAuthenticationScheme returned null."),
                _ => throw new NotSupportedException($"Authentication scheme type '{type}' is not supported.")
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, AuthenticationScheme value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}