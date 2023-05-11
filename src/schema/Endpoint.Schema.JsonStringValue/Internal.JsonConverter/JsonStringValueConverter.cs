using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GarageGroup.Infra;

internal sealed class JsonStringValueConverter<TValue> : JsonConverter<JsonStringValue<TValue>>
    where TValue : struct
{
    public override JsonStringValue<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType is JsonTokenType.String && string.IsNullOrEmpty(reader.GetString()))
        {
            return default;
        }

        return new(JsonSerializer.Deserialize<TValue>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, JsonStringValue<TValue> value, JsonSerializerOptions options)
        =>
        JsonSerializer.Serialize(writer, value.Value, options);
}