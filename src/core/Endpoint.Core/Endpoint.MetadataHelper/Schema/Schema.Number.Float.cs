using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateFloatSchema(bool nullable, IOpenApiAny? example = null)
        =>
        new()
        {
            Type = "number",
            Format = "float",
            Nullable = nullable,
            Example = example
        };
}