using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<decimal, Failure<Unit>> GetDecimalOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDecimalOrFailure);

    public static Result<decimal?, Failure<Unit>> GetNullableDecimalOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDecimalOrFailure);

    private static Result<decimal, Failure<Unit>> GetDecimalOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetDecimal(out var value) ? value : CreateFailure(propertyName, nameof(Decimal));
}