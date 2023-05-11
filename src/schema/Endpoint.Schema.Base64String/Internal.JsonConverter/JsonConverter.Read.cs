using System;
using System.Text.Json;

namespace GarageGroup.Infra;

partial class Base64StringJsonConverter
{
    public override Base64String Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType is not JsonTokenType.String)
        {
            throw new JsonException();
        }

        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return default;
        }

        return new(value);
    }
}