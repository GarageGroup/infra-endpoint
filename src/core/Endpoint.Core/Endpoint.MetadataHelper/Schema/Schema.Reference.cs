using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateReferenceSchema(bool nullable, string typeName, string? description = null)
        =>
        new()
        {
            Nullable = nullable,
            Reference = new()
            {
                Type = ReferenceType.Schema,
                Id = typeName
            },
            Description = description
        };
}