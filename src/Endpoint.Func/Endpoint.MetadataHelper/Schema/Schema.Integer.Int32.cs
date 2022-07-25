using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateInt32Schema(bool nullable)
        =>
        new()
        {
            Type = "integer",
            Format = "int32",
            Nullable = nullable
        };
}