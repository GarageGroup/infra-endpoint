using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateBooleanSchema(bool nullable)
        =>
        new()
        {
            Type = "boolean",
            Nullable = nullable
        };
}