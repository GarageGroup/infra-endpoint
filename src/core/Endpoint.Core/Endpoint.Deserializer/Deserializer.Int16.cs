using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<short, Failure<Unit>> GetInt16OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetInt16OrFailure);

    public static Result<short?, Failure<Unit>> GetNullableInt16OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetInt16OrFailure);

    private static Result<short, Failure<Unit>> GetInt16OrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is JsonValueKind.Number)
        {
            return jsonElement.TryGetInt16(out var value) ? value : CreateParserFailure(propertyName, nameof(Int16));
        }

        if (jsonElement.ValueKind is JsonValueKind.String)
        {
            var text = jsonElement.GetString() ?? string.Empty;
            return short.TryParse(text, out var value) ? value : CreateParserFailure(propertyName, nameof(Int16));
        }

        return CreateValueKindFailure(propertyName, JsonValueKind.Number, JsonValueKind.String);
    }
}