using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<long, Failure<Unit>> GetInt64OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetInt64OrFailure);

    public static Result<long?, Failure<Unit>> GetNullableInt64OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetInt64OrFailure);

    private static Result<long, Failure<Unit>> GetInt64OrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is JsonValueKind.Number)
        {
            return jsonElement.TryGetInt64(out var value) ? value : CreateParserFailure(propertyName, nameof(Int64));
        }

        if (jsonElement.ValueKind is JsonValueKind.String)
        {
            var text = jsonElement.GetString() ?? string.Empty;
            return long.TryParse(text, out var value) ? value : CreateParserFailure(propertyName, nameof(Int64));
        }

        return CreateValueKindFailure(propertyName, JsonValueKind.Number, JsonValueKind.String);
    }
}