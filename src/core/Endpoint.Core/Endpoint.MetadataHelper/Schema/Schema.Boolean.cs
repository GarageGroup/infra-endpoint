using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateBooleanSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "boolean",
            Nullable = nullable,
            Example = example
        };
}