using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateNumberSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "number",
            Nullable = nullable,
            Example = example
        };
}