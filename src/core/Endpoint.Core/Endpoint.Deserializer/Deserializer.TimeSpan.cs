using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<TimeSpan, Failure<Unit>> GetTimeSpanOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetValue(property, GetTimeSpanOrFailure);

    public static Result<TimeSpan?, Failure<Unit>> GetNullableTimeSpanOrFailure(this JsonDocument? document, [AllowNull] string property)
        =>
        document.GetNullableValue(property, GetTimeSpanOrFailure);

    private static Result<TimeSpan, Failure<Unit>> GetTimeSpanOrFailure(JsonElement jsonElement, string propertyName)
    {
        if (jsonElement.ValueKind is not JsonValueKind.String)
        {
            return CreateValueKindFailure(propertyName, JsonValueKind.String);
        }

        return TimeSpan.TryParse(jsonElement.GetString(), out var value) ? value : CreateParserFailure(propertyName, nameof(TimeSpan));
    }
}