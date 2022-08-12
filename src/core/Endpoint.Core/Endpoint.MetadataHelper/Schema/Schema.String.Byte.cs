using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateByteSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Format = "byte",
            Nullable = nullable,
            Example = example
        };
}