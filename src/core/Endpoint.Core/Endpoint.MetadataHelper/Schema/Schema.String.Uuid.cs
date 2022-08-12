using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateUuidSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Format = "uuid",
            Nullable = nullable,
            Example = example
        };
}