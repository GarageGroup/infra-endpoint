using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateStringSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Nullable = nullable
        };
}