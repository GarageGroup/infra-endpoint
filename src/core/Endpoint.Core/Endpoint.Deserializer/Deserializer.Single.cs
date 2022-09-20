using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<float, Failure<Unit>> GetSingleOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetSingleOrFailure);

    public static Result<float?, Failure<Unit>> GetNullableSingleOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetSingleOrFailure);

    private static Result<float, Failure<Unit>> GetSingleOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is JsonValueKind.Number)
        {
            return jsonElement.TryGetSingle(out var value) ? value : CreateParserFailure(propertyName, nameof(Single));
        }

        if (jsonElement.ValueKind is JsonValueKind.String)
        {
            var text = jsonElement.GetString() ?? string.Empty;
            return float.TryParse(text, out var value) ? value : CreateParserFailure(propertyName, nameof(Single));
        }

        return CreateValueKindFailure(propertyName, JsonValueKind.Number, JsonValueKind.String);
    }
}