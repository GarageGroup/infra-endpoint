using Microsoft.OpenApi;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    public static IOpenApiSchema CreateReferenceSchema(bool nullable, string typeName, string? description = null)
        =>
        new OpenApiSchemaReference(
            referenceId: typeName,
            hostDocument: new(),
            externalResource: null)
        {
            Description = description
        };
}
