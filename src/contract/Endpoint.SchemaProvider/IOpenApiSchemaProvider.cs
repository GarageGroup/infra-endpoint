using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace GarageGroup.Infra;

public interface IOpenApiSchemaProvider
{
    static abstract OpenApiSchema GetSchema(bool nullable, JsonNode? example = null, string? description = null);
}