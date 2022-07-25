using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDoubleSchema(bool nullable)
        =>
        new()
        {
            Type = "number",
            Format = "double",
            Nullable = nullable
        };
}