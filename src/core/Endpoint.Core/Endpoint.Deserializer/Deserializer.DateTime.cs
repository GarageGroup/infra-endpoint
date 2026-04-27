using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<DateTime, Failure<Unit>> GetDateTimeOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDateTimeOrFailure);

    public static Result<DateTime?, Failure<Unit>> GetNullableDateTimeOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDateTimeOrFailure);

    private static Result<DateTime, Failure<Unit>> GetDateTimeOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.String)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.String);
        }

        return jsonElement.TryGetDateTime(out var value) ? value : CreateParserFailure(propertyName, nameof(DateTime));
    }
}