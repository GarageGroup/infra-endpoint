using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

public static partial class EndpointDeserializer
{
    private static Result<T, Failure<Unit>> GetValue<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, Func<JsonElement, string, Result<T, Failure<Unit>>> parser)
    {
        var name = propertyName ?? string.Empty;

        var jsonProperty = jsonDocument.GetPropertyOrNull(name);
        if (jsonProperty is null)
        {
            return default(T)!;
        }

        return parser.Invoke(jsonProperty.Value, name);
    }

    private static Result<T?, Failure<Unit>> GetNullableValue<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, Func<JsonElement, string, Result<T, Failure<Unit>>> parser)
        where T : struct
    {
        var name = propertyName ?? string.Empty;

        var jsonProperty = jsonDocument.GetPropertyOrNull(name);
        if (jsonProperty is null || jsonProperty.Value.ValueKind is JsonValueKind.Null)
        {
            return null;
        }

        return parser.Invoke(jsonProperty.Value, name).MapSuccess(ToNullable);

        static T? ToNullable(T value) => value;
    }

    private static JsonElement? GetPropertyOrNull(this JsonDocument? jsonDocument, string propertyName)
    {
        if (jsonDocument is null)
        {
            return null;
        }

        return jsonDocument.RootElement.TryGetProperty(propertyName, out var jsonElement) ? jsonElement : null;
    }

    private static Failure<Unit> CreateValueKindFailure(string propertyName, JsonValueKind valueKind, JsonValueKind? other = null)
    {
        var valueKindText = other switch
        {
            null => ToString(valueKind),
            _ => $"either {ToString(valueKind)} or {ToString(other.Value)}"
        };

        return Failure.Create($"JSON property '{propertyName}' value kind must be {valueKindText}");

        static string ToString(JsonValueKind valueKind)
            =>
            valueKind.ToString("G");
    }

    private static Failure<Unit> CreateParserFailure(string propertyName, string type)
        =>
        Failure.Create($"JSON Property '{propertyName}' was not deserialized as a {type} value");
}