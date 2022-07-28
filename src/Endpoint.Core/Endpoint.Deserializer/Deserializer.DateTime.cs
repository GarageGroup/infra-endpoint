using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<DateTime, Failure<Unit>> GetDateTimeOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDateTimeOrFailure);

    public static Result<DateTime?, Failure<Unit>> GetNullableDateTimeOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDateTimeOrFailure);

    private static Result<DateTime, Failure<Unit>> GetDateTimeOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetDateTime(out var value) ? value : CreateFailure(propertyName, nameof(DateTime));
}