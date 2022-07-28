using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateNumberSchema(bool nullable)
        =>
        new()
        {
            Type = "number",
            Nullable = nullable
        };
}