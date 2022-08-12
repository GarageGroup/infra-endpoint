using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateStringSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Nullable = nullable,
            Example = example
        };
}