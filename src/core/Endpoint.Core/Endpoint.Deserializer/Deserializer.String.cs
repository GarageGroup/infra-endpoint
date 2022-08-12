using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<string, Failure<Unit>> GetStringOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetStringOrFailure);

    private static Result<string, Failure<Unit>> GetStringOrFailure(JsonElement jsonElement, string propertyName)
        =>
        jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
            _ => CreateValueKindFailure(propertyName, JsonValueKind.String)
        };
}