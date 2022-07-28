using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateDateTimeSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Format = "date-time",
            Nullable = nullable
        };
}