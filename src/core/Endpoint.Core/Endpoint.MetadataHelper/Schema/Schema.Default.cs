using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDefaultSchema(bool nullable, IOpenApiAny? example = null, string? description = null)
        =>
        new()
        {
            Nullable = nullable,
            Type = "object",
            Example = example,
            Description = description
        };
}