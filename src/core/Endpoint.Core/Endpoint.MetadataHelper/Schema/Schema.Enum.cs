using System;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateEnumSchema<T>(bool nullable, JsonNode? example = null, string? description = null)
        where T : struct, Enum
    {
        return new()
        {
            Type = CreateSchemaType(JsonSchemaType.String, nullable),
            Enum = Enum.GetNames<T>().Select(ToOpenApiString).ToArray(),
            Example = example,
            Description = description
        };

        static JsonNode ToOpenApiString(string value)
            =>
            JsonValue.Create(value)!;
    }
}
