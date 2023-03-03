using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateArraySchema(bool nullable, OpenApiSchema itemsSchema, string? description = null)
        =>
        new()
        {
            Type = "array",
            Nullable = nullable,
            Items = itemsSchema,
            Description = description
        };
}