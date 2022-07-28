using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDefaultSchema(bool nullable)
        =>
        new()
        {
            Nullable = nullable,
            Type = "object"
        };
}