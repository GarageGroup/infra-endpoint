using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<double, Failure<Unit>> GetDoubleOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetDoubleOrFailure);

    public static Result<double?, Failure<Unit>> GetNullableDoubleOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetDoubleOrFailure);

    private static Result<double, Failure<Unit>> GetDoubleOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetDouble(out var value) ? value : CreateFailure(propertyName, nameof(Double));
}