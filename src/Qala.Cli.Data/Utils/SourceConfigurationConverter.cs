using System.Text.Json;
using System.Text.Json.Serialization;
using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Utils;

public class SourceConfigurationConverter : JsonConverter<SourceConfiguration>
{
    public override SourceConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonString = reader.GetString();
        if (jsonString == null)
        {
            throw new JsonException("Failed to read SourceConfiguration JSON string.");
        }

        return JsonSerializer.Deserialize<SourceConfiguration>(jsonString, options) ?? new SourceConfiguration();
    }

    public override void Write(Utf8JsonWriter writer, SourceConfiguration value, JsonSerializerOptions options)
    {
        var jsonString = JsonSerializer.Serialize(value, options);
        writer.WriteStringValue(jsonString);
    }
}