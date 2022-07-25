using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Format = "date",
            Nullable = nullable
        };
}