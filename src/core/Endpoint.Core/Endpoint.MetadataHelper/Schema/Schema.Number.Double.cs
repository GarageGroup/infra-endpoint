using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDoubleSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "number",
            Format = "double",
            Nullable = nullable,
            Example = example
        };
}