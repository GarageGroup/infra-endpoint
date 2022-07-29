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
        if (jsonElement.ValueKind is not JsonValueKind.Number)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.Number);
        }

        return jsonElement.TryGetSingle(out var value) ? value : CreateParserFailure(propertyName, nameof(Single));
    }
}