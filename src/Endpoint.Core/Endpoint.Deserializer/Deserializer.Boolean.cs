using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<bool, Failure<Unit>> GetBooleanOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetBooleanOrFailure);

    public static Result<bool?, Failure<Unit>> GetNullableBooleanOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetBooleanOrFailure);

    private static Result<bool, Failure<Unit>> GetBooleanOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => CreateValueKindFailure(propertyName, JsonValueKind.True, JsonValueKind.False)
        };
}