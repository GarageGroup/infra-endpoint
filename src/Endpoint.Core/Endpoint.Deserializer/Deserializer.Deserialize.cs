using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<T, Failure<Unit>> DeserializeOrFailure<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, JsonSerializerOptions? jsonSerializerOptions)
        =>
        jsonDocument.DeserializeOrFailure<T>(propertyName, jsonSerializerOptions, null);

    public static Result<T, Failure<Unit>> DeserializeOrFailure<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, JsonSerializerOptions? jsonSerializerOptions, ILogger? logger)
    {
        return jsonDocument.GetValue(propertyName, InnerDeserialize);

        Result<T, Failure<Unit>> InnerDeserialize(JsonElement property, string name)
        {
            try
            {
                return property.Deserialize<T>(jsonSerializerOptions)!;
            }
            catch (Exception exception)
            {
                logger?.LogDebug(exception, "Request body property '{name}' is incorrect. Request value: '{value}'", name, property.GetRawText());
                return Failure.Create($"Request body property '{name}' is incorrect");
            }
        }
    }
}