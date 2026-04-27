using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<Guid, Failure<Unit>> GetGuidOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetGuidOrFailure);

    public static Result<Guid?, Failure<Unit>> GetNullableGuidOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetGuidOrFailure);

    private static Result<Guid, Failure<Unit>> GetGuidOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.String)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.String);
        }

        return jsonElement.TryGetGuid(out var value) ? value : CreateParserFailure(propertyName, nameof(Guid));
    }
}