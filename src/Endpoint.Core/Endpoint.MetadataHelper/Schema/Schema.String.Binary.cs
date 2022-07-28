using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateBinarySchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Format = "binary",
            Nullable = nullable
        };
}