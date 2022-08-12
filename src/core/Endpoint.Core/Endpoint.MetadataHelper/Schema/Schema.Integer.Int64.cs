using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateInt64Schema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "integer",
            Format = "int64",
            Nullable = nullable,
            Example = example
        };
}