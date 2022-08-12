using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateBinarySchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Format = "binary",
            Nullable = nullable,
            Example = example
        };
}