using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateIntegerSchema(bool nullable)
        =>
        new()
        {
            Type = "integer",
            Nullable = nullable
        };
}