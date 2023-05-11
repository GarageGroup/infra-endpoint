using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GarageGroup.Infra.Endpoint;

public sealed class EndpointEnumJsonConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => reader.GetString()
        };

        return EndpointParser.ParseEnum<T>(text).SuccessOrThrow(CreateParserException);

        JsonException CreateParserException()
            =>
            new($"An unexpected {typeof(T)} value: '{text}'");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var name = Enum.GetName(value);
        writer.WriteStringValue(name);
    }
}