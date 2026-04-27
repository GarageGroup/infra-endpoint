using System.Text.Json.Nodes;
using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateInt64Schema(bool nullable, JsonNode? example = null, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.Integer, nullable),
            Format = "int64",
            Example = example,
            Description = description
        };
}