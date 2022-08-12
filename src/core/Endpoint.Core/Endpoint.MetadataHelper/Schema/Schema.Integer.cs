using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateIntegerSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "integer",
            Nullable = nullable,
            Example = example
        };
}