using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateUuidSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Format = "uuid",
            Nullable = nullable
        };
}