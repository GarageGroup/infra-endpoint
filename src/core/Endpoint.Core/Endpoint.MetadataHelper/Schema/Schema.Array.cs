using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateArraySchema(bool nullable, OpenApiSchema itemsSchema, string? description = null)
        =>
        new()
        {
            Type = CreateSchemaType(JsonSchemaType.Array, nullable),
            Items = itemsSchema,
            Description = description
        };
}