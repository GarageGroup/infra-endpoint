using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<short, Failure<Unit>> GetInt16OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetInt16OrFailure);

    public static Result<short?, Failure<Unit>> GetNullableInt16OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetInt16OrFailure);

    private static Result<short, Failure<Unit>> GetInt16OrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetInt16(out var value) ? value : CreateFailure(propertyName, nameof(Int16));
}