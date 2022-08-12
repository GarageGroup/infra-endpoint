using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateArraySchema(bool nullable, OpenApiSchema itemsSchema)
        =>
        new()
        {
            Type = "array",
            Nullable = nullable,
            Items = itemsSchema
        };
}