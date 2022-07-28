using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<T, Failure<Unit>> DeserializeOrFailure<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, ILogger? logger, JsonSerializerOptions? jsonSerializerOptions)
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
                logger?.LogError(exception, "An unexpected error occured when the request body was being deserialized");
                return Failure.Create($"Request body property '{name}' is incorrect");
            }
        }
    }
}