using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static OpenApiSchema CreateReferenceSchema(bool nullable, string typeName)
        =>
        new()
        {
            Nullable = nullable,
            Reference = new()
            {
                Type = ReferenceType.Schema,
                Id = typeName
            }
        };
}