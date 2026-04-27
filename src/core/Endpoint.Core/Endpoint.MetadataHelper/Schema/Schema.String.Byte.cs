using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateByteSchema(bool nullable, JsonNode? example = null, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.String, nullable),
            Format = "byte",
            Example = example,
            Description = description
        };
}