using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<TimeOnly, Failure<Unit>> GetTimeOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetTimeOnlyOrFailure);

    public static Result<TimeOnly?, Failure<Unit>> GetNullableTimeOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetTimeOnlyOrFailure);

    private static Result<TimeOnly, Failure<Unit>> GetTimeOnlyOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.String)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.String);
        }

        return jsonElement.TryGetDateTime(out var value) ? TimeOnly.FromDateTime(value) : CreateParserFailure(propertyName, nameof(TimeOnly));
    }
}