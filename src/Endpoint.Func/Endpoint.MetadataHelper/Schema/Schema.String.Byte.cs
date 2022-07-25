using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateByteSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Format = "byte",
            Nullable = nullable
        };
}