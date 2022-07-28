using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<DateTimeOffset, Failure<Unit>> GetDateTimeOffsetOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDateTimeOffsetOrFailure);

    public static Result<DateTimeOffset?, Failure<Unit>> GetNullableDateTimeOffsetOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDateTimeOffsetOrFailure);

    private static Result<DateTimeOffset, Failure<Unit>> GetDateTimeOffsetOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetDateTimeOffset(out var value) ? value : CreateFailure(propertyName, nameof(DateTimeOffset));
}