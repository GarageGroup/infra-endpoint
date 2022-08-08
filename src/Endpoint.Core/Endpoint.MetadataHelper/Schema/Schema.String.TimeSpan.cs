using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateTimeSpanSchema(bool nullable)
        =>
        new()
        {
            Type = "string",
            Example = new OpenApiString("00:00:00"),
            Nullable = nullable
        };
}