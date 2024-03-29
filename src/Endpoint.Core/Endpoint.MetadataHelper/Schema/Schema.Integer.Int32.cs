using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateInt32Schema(bool nullable, IOpenApiAny? example = null, string? description = null)
        =>
        new()
        {
            Type = "integer",
            Format = "int32",
            Nullable = nullable,
            Example = example,
            Description = description
        };
}