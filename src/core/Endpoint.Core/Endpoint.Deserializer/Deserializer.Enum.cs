using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<TEnum, Failure<Unit>> GetEnumOrFailure<TEnum>(this JsonDocument? document, [AllowNull] string property)
        where TEnum : struct, Enum
        =>
        document.GetValue(property, GetEnumOrFailure<TEnum>);

    public static Result<TEnum?, Failure<Unit>> GetNullableEnumOrFailure<TEnum>(this JsonDocument? document, [AllowNull] string property)
        where TEnum : struct, Enum
        =>
        document.GetNullableValue(property, GetEnumOrFailure<TEnum>);

    private static Result<TEnum, Failure<Unit>> GetEnumOrFailure<TEnum>(JsonElement jsonElement, string propertyName)
        where TEnum : struct, Enum
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => Parse(jsonElement.GetString()),
            JsonValueKind.Number => Parse(jsonElement.GetRawText()),
            _ => CreateValueKindFailure(propertyName, JsonValueKind.String, JsonValueKind.Number)
        };

        Result<TEnum, Failure<Unit>> Parse(string? source)
            =>
            Enum.TryParse<TEnum>(source, true, out var value) ? value : CreateParserFailure(propertyName, typeof(TEnum).Name);
    }
}