using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateTimeSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "string",
            Format = "date-time",
            Nullable = nullable,
            Example = example
        };
}