using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDoubleSchema(bool nullable, JsonNode? example = null, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.Number, nullable),
            Format = "double",
            Example = example,
            Description = description
        };
}