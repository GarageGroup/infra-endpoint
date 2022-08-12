using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<byte, Failure<Unit>> GetByteOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetByteOrFailure);

    public static Result<byte?, Failure<Unit>> GetNullableByteOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetByteOrFailure);

    private static Result<byte, Failure<Unit>> GetByteOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.Number)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.Number);
        }

        return jsonElement.TryGetByte(out var value) ? value : CreateParserFailure(propertyName, nameof(Byte));
    }
}