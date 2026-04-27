using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDefaultSchema(bool nullable, JsonNode? example = null, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.Object, nullable),
            Example = example,
            Description = description
        };
}