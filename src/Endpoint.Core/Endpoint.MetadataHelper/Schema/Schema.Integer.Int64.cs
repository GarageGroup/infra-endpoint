using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateInt64Schema(bool nullable)
        =>
        new()
        {
            Type = "integer",
            Format = "int64",
            Nullable = nullable
        };
}