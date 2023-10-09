using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static Result<T, Failure<Unit>> DeserializeOrFailure<T>(
        this JsonDocument? jsonDocument, [AllowNull] string propertyName, JsonSerializerOptions? jsonSerializerOptions)
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
                return exception.ToFailure($"Request body property '{name}' is incorrect");
            }
        }
    }
}