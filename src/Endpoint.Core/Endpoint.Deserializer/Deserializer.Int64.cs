using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<long, Failure<Unit>> GetInt64OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetInt64OrFailure);

    public static Result<long?, Failure<Unit>> GetNullableInt64OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetInt64OrFailure);

    private static Result<long, Failure<Unit>> GetInt64OrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetInt64(out var value) ? value : CreateFailure(propertyName, nameof(Int64));
}