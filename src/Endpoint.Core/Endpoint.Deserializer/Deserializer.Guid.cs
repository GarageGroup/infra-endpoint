using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<Guid, Failure<Unit>> GetGuidOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetGuidOrFailure);

    public static Result<Guid?, Failure<Unit>> GetNullableGuidOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetGuidOrFailure);

    private static Result<Guid, Failure<Unit>> GetGuidOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.TryGetGuid(out var value) ? value : CreateFailure(propertyName, nameof(Guid));
}