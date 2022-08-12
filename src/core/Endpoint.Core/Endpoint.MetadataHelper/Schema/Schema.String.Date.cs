using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Format = "date",
            Nullable = nullable,
            Example = example
        };
}