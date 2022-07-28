using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateFloatSchema(bool nullable)
        =>
        new()
        {
            Type = "number",
            Format = "float",
            Nullable = nullable
        };
}