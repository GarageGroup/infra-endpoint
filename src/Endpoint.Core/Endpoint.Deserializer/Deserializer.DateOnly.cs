using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<DateOnly, Failure<Unit>> GetDateOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDateOnlyOrFailure);

    public static Result<DateOnly?, Failure<Unit>> GetNullableDateOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDateOnlyOrFailure);

    private static Result<DateOnly, Failure<Unit>> GetDateOnlyOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.String)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.String);
        }

        return jsonElement.TryGetDateTime(out var value) ? DateOnly.FromDateTime(value) : CreateParserFailure(propertyName, nameof(DateOnly));
    }
}