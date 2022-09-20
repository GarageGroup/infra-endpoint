using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<int, Failure<Unit>> GetInt32OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetInt32OrFailure);

    public static Result<int?, Failure<Unit>> GetNullableInt32OrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetInt32OrFailure);

    private static Result<int, Failure<Unit>> GetInt32OrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is JsonValueKind.Number)
        {
            return jsonElement.TryGetInt32(out var value) ? value : CreateParserFailure(propertyName, nameof(Int32));
        }

        if (jsonElement.ValueKind is JsonValueKind.String)
        {
            var text = jsonElement.GetString() ?? string.Empty;
            return int.TryParse(text, out var value) ? value : CreateParserFailure(propertyName, nameof(Int32));
        }

        return CreateValueKindFailure(propertyName, JsonValueKind.Number, JsonValueKind.String);
    }
}