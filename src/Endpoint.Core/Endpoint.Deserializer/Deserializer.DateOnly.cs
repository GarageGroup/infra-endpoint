using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<DateOnly, Failure<Unit>> GetDateOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDateOnlyOrFailure);

    public static Result<DateOnly?, Failure<Unit>> GetNullableDateOnlyOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDateOnlyOrFailure);

    private static Result<DateOnly, Failure<Unit>> GetDateOnlyOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetDateTime(out var value) ? DateOnly.FromDateTime(value) : CreateFailure(propertyName, nameof(DateOnly));
}